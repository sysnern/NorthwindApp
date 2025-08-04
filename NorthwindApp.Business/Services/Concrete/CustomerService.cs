using AutoMapper;
using NorthwindApp.Business.Services.Abstract;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;
using NorthwindApp.Data.Repositories.Abstract;
using NorthwindApp.Entities.Models;
using System.Linq.Expressions;

namespace NorthwindApp.Business.Services.Concrete
{
    public class CustomerService : GenericService<Customer, CustomerDTO, CustomerCreateDto, CustomerUpdateDto, string>, ICustomerService
    {
        public CustomerService(
            ICustomerRepository repo,
            IMapper mapper,
            ICacheService cacheService)
            : base(repo, mapper, cacheService, "customer_list_", "Müşteri")
        {
        }

        protected override string GetIdFromUpdateDto(CustomerUpdateDto dto)
        {
            return dto.CustomerId;
        }

        // Override to handle CustomerFilterDto specifically
        public async Task<ApiResponse<List<CustomerDTO>>> GetAllAsync(CustomerFilterDto? filter = null)
        {
            // Build filter expression
            Expression<Func<Customer, bool>>? filterExpression = null;
            
            if (filter != null)
            {
                filterExpression = c =>
                    (string.IsNullOrEmpty(filter.CustomerId) || (c.CustomerId != null && c.CustomerId.Contains(filter.CustomerId))) &&
                    (string.IsNullOrEmpty(filter.CompanyName) || (c.CompanyName != null && c.CompanyName.Contains(filter.CompanyName))) &&
                    (string.IsNullOrEmpty(filter.ContactName) || (c.ContactName != null && c.ContactName.Contains(filter.ContactName))) &&
                    (string.IsNullOrEmpty(filter.City) || (c.City != null && c.City.Contains(filter.City))) &&
                    (string.IsNullOrEmpty(filter.Country) || (c.Country != null && c.Country.Contains(filter.Country))) &&
                    (!filter.IsDeleted.HasValue || c.IsDeleted == filter.IsDeleted);
            }

            return await base.GetAllAsync(filterExpression);
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
        protected override async Task<BusinessValidationResult> ValidateBusinessRulesForCreate(CustomerCreateDto dto)
        {
            // Example business rule: Customer ID must be unique
            // This would typically check against the database
            return BusinessValidationResult.Success();
        }

        protected override async Task<BusinessValidationResult> ValidateBusinessRulesForUpdate(CustomerUpdateDto dto, Customer entity)
        {
            // Example business rule: Cannot update deleted customers
            if (entity.IsDeleted)
            {
                return BusinessValidationResult.Failure("Silinmiş müşteri güncellenemez.");
            }
            return BusinessValidationResult.Success();
        }
    }
}
