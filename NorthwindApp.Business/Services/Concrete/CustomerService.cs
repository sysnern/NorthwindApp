using AutoMapper;
using Microsoft.Extensions.Logging;
using NorthwindApp.Business.Services.Abstract;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;
using NorthwindApp.Data.Repositories.Abstract;
using NorthwindApp.Entities.Models;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;

namespace NorthwindApp.Business.Services.Concrete
{
    public class CustomerService : GenericService<Customer, CustomerDTO, CustomerCreateDto, CustomerUpdateDto, string>, ICustomerService
    {
        public CustomerService(
            ICustomerRepository customerRepo,
            IMapper mapper,
            ICacheService cacheService,
            ILogger<CustomerService> logger,
            IHttpContextAccessor httpContextAccessor)
            : base(customerRepo, mapper, cacheService, logger, httpContextAccessor, "customer_list_", "Müşteri")
        {
        }

        // Override to handle CustomerFilterDto specifically with pagination and sorting
        public async Task<ApiResponse<List<CustomerDTO>>> GetAllAsync(CustomerFilterDto? filter = null)
        {
            // Build filter expression
            Expression<Func<Customer, bool>>? filterExpression = null;
            
            if (filter != null)
            {
                filterExpression = c =>
                (string.IsNullOrEmpty(filter.CustomerId) || (c.CustomerId != null && c.CustomerId.ToLower().Contains(filter.CustomerId.ToLower()))) &&
                (string.IsNullOrEmpty(filter.CompanyName) || (c.CompanyName != null && c.CompanyName.ToLower().Contains(filter.CompanyName.ToLower()))) &&
                (string.IsNullOrEmpty(filter.ContactName) || (c.ContactName != null && c.ContactName.ToLower().Contains(filter.ContactName.ToLower()))) &&
                (string.IsNullOrEmpty(filter.City) || (c.City != null && c.City.ToLower().Contains(filter.City.ToLower()))) &&
                (string.IsNullOrEmpty(filter.Country) || (c.Country != null && c.Country.ToLower().Contains(filter.Country.ToLower()))) &&
                (!filter.IsDeleted.HasValue || c.IsDeleted == filter.IsDeleted);
            }

            // Extract pagination and sorting parameters from filter
            var sortField = filter?.SortField ?? "CustomerId";
            var sortDirection = filter?.SortDirection ?? "asc";
            var page = filter?.Page ?? 1;
            var pageSize = filter?.PageSize ?? 10;

            // Generate cache key with filter parameters
            var cacheKey = GenerateCacheKeyWithFilter(filter);

            return await base.GetAllAsync(filterExpression, sortField, sortDirection, page, pageSize, cacheKey);
        }

        private string GenerateCacheKeyWithFilter(CustomerFilterDto? filter)
        {
            if (filter == null)
                return _cachePrefix;

            // Create a unique cache key based on filter parameters
            var filterParams = new List<string>
            {
                $"cid:{filter.CustomerId ?? ""}",
                $"cn:{filter.CompanyName ?? ""}",
                $"ctn:{filter.ContactName ?? ""}",
                $"city:{filter.City ?? ""}",
                $"country:{filter.Country ?? ""}",
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

        protected override string GetIdFromUpdateDto(CustomerUpdateDto dto)
        {
            return dto.CustomerId;
        }

        protected override bool SupportsSoftDelete() => true;

        protected override void PerformSoftDelete(Customer entity)
        {
            entity.IsDeleted = true;
        }



        // Implement ICustomerService methods that return string instead of CustomerDTO
        async Task<ApiResponse<string>> ICustomerService.AddAsync(CustomerCreateDto dto)
        {
            var result = await base.AddAsync(dto);
            if (result.Success)
            {
                return ApiResponse<string>.Created(null, result.Message);
            }
            return ApiResponse<string>.BadRequest(result.Errors ?? new[] { result.Message }, result.Message);
        }

        async Task<ApiResponse<string>> ICustomerService.UpdateAsync(CustomerUpdateDto dto)
        {
            var result = await base.UpdateAsync(dto);
            if (result.Success)
            {
                return ApiResponse<string>.Ok(null, result.Message);
            }
            return ApiResponse<string>.BadRequest(result.Errors ?? new[] { result.Message }, result.Message);
        }

        public override async Task<ApiResponse<string>> DeleteAsync(string id)
        {
            var result = await base.DeleteAsync(id);
            if (result.Success)
            {
                return ApiResponse<string>.NoContent(result.Message);
            }
            return ApiResponse<string>.BadRequest(result.Errors ?? new[] { result.Message }, result.Message);
        }

        // Business rule validation
        protected override Task<BusinessValidationResult> ValidateBusinessRulesForCreate(CustomerCreateDto dto)
        {
            // Example business rule: Customer ID must be unique
            // This would typically check against the database
            return Task.FromResult(BusinessValidationResult.Success());
        }

        protected override Task<BusinessValidationResult> ValidateBusinessRulesForUpdate(CustomerUpdateDto dto, Customer entity)
        {
            // Example business rule: Cannot update deleted customers
            if (entity.IsDeleted)
            {
                return Task.FromResult(BusinessValidationResult.Failure("Silinmiş müşteri güncellenemez."));
            }
            return Task.FromResult(BusinessValidationResult.Success());
        }
    }
}
