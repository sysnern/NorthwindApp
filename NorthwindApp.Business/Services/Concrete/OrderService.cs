using AutoMapper;
using Microsoft.Extensions.Logging;
using NorthwindApp.Business.Services.Abstract;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;
using NorthwindApp.Data.Repositories.Abstract;
using NorthwindApp.Entities.Models;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;

namespace NorthwindApp.Business.Services.Concrete
{
    public class OrderService : GenericService<Order, OrderDTO, OrderCreateDto, OrderUpdateDto, int>, IOrderService
    {
        public OrderService(
            IOrderRepository orderRepo,
            IMapper mapper,
            ICacheService cacheService,
            ILogger<OrderService> logger,
            IHttpContextAccessor httpContextAccessor)
            : base(orderRepo, mapper, cacheService, logger, httpContextAccessor, "order_list_", "Sipariş")
        {
        }

        // Override to handle OrderFilterDto specifically with pagination and sorting
        public async Task<ApiResponse<List<OrderDTO>>> GetAllAsync(OrderFilterDto? filter = null)
        {
            // Build filter expression
            Expression<Func<Order, bool>>? filterExpression = null;
            
            if (filter != null)
            {
                filterExpression = o =>
                (!filter.OrderId.HasValue || o.OrderId == filter.OrderId) &&
                (string.IsNullOrEmpty(filter.CustomerId) || (o.CustomerId != null && o.CustomerId.ToLower().Contains(filter.CustomerId.ToLower()))) &&
                (!filter.EmployeeId.HasValue || o.EmployeeId == filter.EmployeeId) &&
                (!filter.OrderDateFrom.HasValue || o.OrderDate >= filter.OrderDateFrom) &&
                (!filter.OrderDateTo.HasValue || o.OrderDate <= filter.OrderDateTo) &&
                (!filter.IsDeleted.HasValue || o.IsDeleted == filter.IsDeleted);
            }

            // Extract pagination and sorting parameters from filter
            var sortField = filter?.SortField ?? "OrderId";
            var sortDirection = filter?.SortDirection ?? "asc";
            var page = filter?.Page ?? 1;
            var pageSize = filter?.PageSize ?? 10;

            // Generate cache key with filter parameters
            var cacheKey = GenerateCacheKeyWithFilter(filter);

            return await base.GetAllAsync(filterExpression, sortField, sortDirection, page, pageSize, cacheKey);
        }

        private string GenerateCacheKeyWithFilter(OrderFilterDto? filter)
        {
            if (filter == null)
                return _cachePrefix;

            // Create a unique cache key based on filter parameters
            var filterParams = new List<string>
            {
                $"oid:{filter.OrderId ?? 0}",
                $"cid:{filter.CustomerId ?? ""}",
                $"eid:{filter.EmployeeId ?? 0}",
                $"odf:{filter.OrderDateFrom?.ToString("yyyyMMdd") ?? ""}",
                $"odt:{filter.OrderDateTo?.ToString("yyyyMMdd") ?? ""}",
                $"del:{filter.IsDeleted?.ToString() ?? ""}",
                $"sort:{filter.SortField ?? ""}",
                $"dir:{filter.SortDirection ?? ""}",
                $"page:{filter.Page}",
                $"size:{filter.PageSize}"
            };

            var filterString = string.Join("_", filterParams);
            var hash = filterString.GetHashCode();
            return $"{_cachePrefix}_{hash}";
        }

        protected override int GetIdFromUpdateDto(OrderUpdateDto dto)
        {
            return dto.OrderId;
        }

        protected override bool SupportsSoftDelete() => true;

        protected override void PerformSoftDelete(Order entity)
        {
            entity.IsDeleted = true;
        }



        // Implement IOrderService methods that return string instead of OrderDTO
        async Task<ApiResponse<string>> IOrderService.AddAsync(OrderCreateDto dto)
        {
            var result = await base.AddAsync(dto);
            if (result.Success)
            {
                return ApiResponse<string>.Created(null, result.Message);
            }
            return ApiResponse<string>.BadRequest(result.Errors ?? new[] { result.Message }, result.Message);
        }

        async Task<ApiResponse<string>> IOrderService.UpdateAsync(OrderUpdateDto dto)
        {
            var result = await base.UpdateAsync(dto);
            if (result.Success)
            {
                return ApiResponse<string>.Ok(null, result.Message);
            }
            return ApiResponse<string>.BadRequest(result.Errors ?? new[] { result.Message }, result.Message);
        }

        public override async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var result = await base.DeleteAsync(id);
            if (result.Success)
            {
                return ApiResponse<string>.NoContent(result.Message);
            }
            return ApiResponse<string>.BadRequest(result.Errors ?? new[] { result.Message }, result.Message);
        }

        // Business rule validation
        protected override Task<BusinessValidationResult> ValidateBusinessRulesForCreate(OrderCreateDto dto)
        {
            // Example business rule: Order must have a customer
            if (string.IsNullOrEmpty(dto.CustomerId))
            {
                return Task.FromResult(BusinessValidationResult.Failure("Sipariş müşterisi boş olamaz."));
            }
            return Task.FromResult(BusinessValidationResult.Success());
        }

        protected override Task<BusinessValidationResult> ValidateBusinessRulesForUpdate(OrderUpdateDto dto, Order entity)
        {
            // Example business rule: Cannot update deleted orders
            if (entity.IsDeleted)
            {
                return Task.FromResult(BusinessValidationResult.Failure("Silinmiş sipariş güncellenemez."));
            }
            return Task.FromResult(BusinessValidationResult.Success());
        }
    }
}
