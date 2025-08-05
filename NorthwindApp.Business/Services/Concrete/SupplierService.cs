using AutoMapper;
using NorthwindApp.Business.Services.Abstract;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;
using NorthwindApp.Data.Repositories.Abstract;
using NorthwindApp.Entities.Models;
using System.Linq.Expressions;

namespace NorthwindApp.Business.Services.Concrete
{
    public class SupplierService : GenericService<Supplier, SupplierDTO, SupplierCreateDto, SupplierUpdateDto, int>, ISupplierService
    {
        public SupplierService(
            ISupplierRepository supplierRepo,
            IMapper mapper,
            ICacheService cacheService)
            : base(supplierRepo, mapper, cacheService, "supplier_list_", "Tedarikçi")
        {
        }

        // Override to handle SupplierFilterDto specifically with pagination and sorting
        public async Task<ApiResponse<List<SupplierDTO>>> GetAllAsync(SupplierFilterDto? filter = null)
        {
            // Build filter expression
            Expression<Func<Supplier, bool>>? filterExpression = null;
            
            if (filter != null && !IsEmptyFilter(filter))
            {
                filterExpression = s =>
                (string.IsNullOrEmpty(filter.CompanyName) || (s.CompanyName != null && s.CompanyName.Contains(filter.CompanyName))) &&
                (string.IsNullOrEmpty(filter.ContactName) || (s.ContactName != null && s.ContactName.Contains(filter.ContactName))) &&
                (string.IsNullOrEmpty(filter.City) || (s.City != null && s.City.Contains(filter.City))) &&
                (string.IsNullOrEmpty(filter.Country) || (s.Country != null && s.Country.Contains(filter.Country)));
            }

            // Extract pagination and sorting parameters from filter
            var sortField = filter?.SortField;
            var sortDirection = filter?.SortDirection;
            var page = filter?.Page ?? 1;
            var pageSize = filter?.PageSize ?? 10;

            return await base.GetAllAsync(filterExpression, sortField, sortDirection, page, pageSize);
        }

        protected override int GetIdFromUpdateDto(SupplierUpdateDto dto)
        {
            return dto.SupplierId;
        }

        protected override bool SupportsSoftDelete() => false;

        private static bool IsEmptyFilter(SupplierFilterDto filter)
        {
            return string.IsNullOrEmpty(filter.CompanyName) &&
                   string.IsNullOrEmpty(filter.ContactName) &&
                   string.IsNullOrEmpty(filter.City) &&
                   string.IsNullOrEmpty(filter.Country);
        }

        // Implement ISupplierService methods that return string instead of SupplierDTO
        async Task<ApiResponse<string>> ISupplierService.AddAsync(SupplierCreateDto dto)
        {
            var result = await base.AddAsync(dto);
            if (result.Success)
            {
                return ApiResponse<string>.Created(null, result.Message);
            }
            return ApiResponse<string>.BadRequest(result.Errors ?? new[] { result.Message }, result.Message);
        }

        async Task<ApiResponse<string>> ISupplierService.UpdateAsync(SupplierUpdateDto dto)
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
        protected override async Task<BusinessValidationResult> ValidateBusinessRulesForCreate(SupplierCreateDto dto)
        {
            // Example business rule: Supplier must have a company name
            if (string.IsNullOrEmpty(dto.CompanyName))
            {
                return BusinessValidationResult.Failure("Tedarikçi şirket adı boş olamaz.");
            }
            return BusinessValidationResult.Success();
        }

        protected override async Task<BusinessValidationResult> ValidateBusinessRulesForUpdate(SupplierUpdateDto dto, Supplier entity)
        {
            // Example business rule: Cannot update deleted suppliers
            if (entity.IsDeleted)
            {
                return BusinessValidationResult.Failure("Silinmiş tedarikçi güncellenemez.");
            }
            return BusinessValidationResult.Success();
        }
    }
}
