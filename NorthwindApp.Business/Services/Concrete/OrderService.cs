using AutoMapper;
using NorthwindApp.Business.Services.Abstract;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;
using NorthwindApp.Data.Repositories.Abstract;
using NorthwindApp.Entities.Models;
using System.Linq.Expressions;

namespace NorthwindApp.Business.Services.Concrete
{
    public class OrderService : GenericService<Order, OrderDTO, OrderCreateDto, OrderUpdateDto, int>, IOrderService
    {
        public OrderService(
            IOrderRepository orderRepo,
            IMapper mapper,
            ICacheService cacheService)
            : base(orderRepo, mapper, cacheService, "order_list_", "Sipariş")
        {
        }

        // Override to handle OrderFilterDto specifically with pagination and sorting
        public async Task<ApiResponse<List<OrderDTO>>> GetAllAsync(OrderFilterDto? filter = null)
        {
            // Build filter expression
            Expression<Func<Order, bool>>? filterExpression = null;
            
            if (filter != null && !IsEmptyFilter(filter))
            {
                filterExpression = o =>
                (string.IsNullOrEmpty(filter.CustomerId) || (o.CustomerId != null && o.CustomerId.Contains(filter.CustomerId))) &&
                (!filter.EmployeeId.HasValue || o.EmployeeId == filter.EmployeeId) &&
                (!filter.OrderDateFrom.HasValue || o.OrderDate >= filter.OrderDateFrom) &&
                (!filter.OrderDateTo.HasValue || o.OrderDate <= filter.OrderDateTo) &&
                (!filter.ShippedDateFrom.HasValue || o.ShippedDate >= filter.ShippedDateFrom) &&
                (!filter.ShippedDateTo.HasValue || o.ShippedDate <= filter.ShippedDateTo) &&
                (string.IsNullOrEmpty(filter.ShipCity) || (o.ShipCity != null && o.ShipCity.Contains(filter.ShipCity))) &&
                (string.IsNullOrEmpty(filter.ShipCountry) || (o.ShipCountry != null && o.ShipCountry.Contains(filter.ShipCountry)));
            }

            // Extract pagination and sorting parameters from filter
            var sortField = filter?.SortField;
            var sortDirection = filter?.SortDirection;
            var page = filter?.Page ?? 1;
            var pageSize = filter?.PageSize ?? 10;

            return await base.GetAllAsync(filterExpression, sortField, sortDirection, page, pageSize);
        }

        protected override int GetIdFromUpdateDto(OrderUpdateDto dto)
        {
            return dto.OrderId;
        }

        protected override bool SupportsSoftDelete() => false;

        private static bool IsEmptyFilter(OrderFilterDto filter)
        {
            return string.IsNullOrEmpty(filter.CustomerId) &&
                   !filter.EmployeeId.HasValue &&
                   !filter.OrderDateFrom.HasValue &&
                   !filter.OrderDateTo.HasValue &&
                   !filter.ShippedDateFrom.HasValue &&
                   !filter.ShippedDateTo.HasValue &&
                   string.IsNullOrEmpty(filter.ShipCity) &&
                   string.IsNullOrEmpty(filter.ShipCountry);
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
        protected override async Task<BusinessValidationResult> ValidateBusinessRulesForCreate(OrderCreateDto dto)
        {
            // Example business rule: Order must have a customer
            if (string.IsNullOrEmpty(dto.CustomerId))
            {
                return BusinessValidationResult.Failure("Sipariş müşterisi boş olamaz.");
            }
            return BusinessValidationResult.Success();
        }

        protected override async Task<BusinessValidationResult> ValidateBusinessRulesForUpdate(OrderUpdateDto dto, Order entity)
        {
            // Example business rule: Cannot update deleted orders
            if (entity.IsDeleted)
            {
                return BusinessValidationResult.Failure("Silinmiş sipariş güncellenemez.");
            }
            return BusinessValidationResult.Success();
        }
    }
}
