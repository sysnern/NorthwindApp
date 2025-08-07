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
    public class SupplierService : GenericService<Supplier, SupplierDTO, SupplierCreateDto, SupplierUpdateDto, int>, ISupplierService
    {
        public SupplierService(
            ISupplierRepository supplierRepo,
            IMapper mapper,
            ICacheService cacheService,
            ILogger<SupplierService> logger,
            IHttpContextAccessor httpContextAccessor)
            : base(supplierRepo, mapper, cacheService, logger, httpContextAccessor, "supplier_list_", "Tedarikçi")
        {
        }

        // Override to handle SupplierFilterDto specifically with pagination and sorting
        public async Task<ApiResponse<List<SupplierDTO>>> GetAllAsync(SupplierFilterDto? filter = null)
        {
            // Build filter expression
            Expression<Func<Supplier, bool>>? filterExpression = null;
            
            if (filter != null)
            {
                filterExpression = s =>
                (string.IsNullOrEmpty(filter.CompanyName) || (s.CompanyName != null && s.CompanyName.ToLower().Contains(filter.CompanyName.ToLower()))) &&
                (string.IsNullOrEmpty(filter.ContactName) || (s.ContactName != null && s.ContactName.ToLower().Contains(filter.ContactName.ToLower()))) &&
                (string.IsNullOrEmpty(filter.City) || (s.City != null && s.City.ToLower().Contains(filter.City.ToLower()))) &&
                (string.IsNullOrEmpty(filter.Country) || (s.Country != null && s.Country.ToLower().Contains(filter.Country.ToLower()))) &&
                (!filter.IsDeleted.HasValue || s.IsDeleted == filter.IsDeleted);
            }

            // Extract pagination and sorting parameters from filter
            var sortField = filter?.SortField ?? "SupplierId";
            var sortDirection = filter?.SortDirection ?? "asc";
            var page = filter?.Page ?? 1;
            var pageSize = filter?.PageSize ?? 10;

            // Generate cache key with filter parameters
            var cacheKey = GenerateCacheKeyWithFilter(filter);

            return await base.GetAllAsync(filterExpression, sortField, sortDirection, page, pageSize, cacheKey);
        }

        private string GenerateCacheKeyWithFilter(SupplierFilterDto? filter)
        {
            if (filter == null)
                return _cachePrefix;

            // Create a unique cache key based on filter parameters
            var filterParams = new List<string>
            {
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

        protected override int GetIdFromUpdateDto(SupplierUpdateDto dto)
        {
            return dto.SupplierId;
        }

        protected override bool SupportsSoftDelete() => true;

        protected override void PerformSoftDelete(Supplier entity)
        {
            entity.IsDeleted = true;
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
        protected override Task<BusinessValidationResult> ValidateBusinessRulesForCreate(SupplierCreateDto dto)
        {
            // Example business rule: Supplier must have a company name
            if (string.IsNullOrEmpty(dto.CompanyName))
            {
                return Task.FromResult(BusinessValidationResult.Failure("Tedarikçi şirket adı boş olamaz."));
            }
            return Task.FromResult(BusinessValidationResult.Success());
        }

        protected override Task<BusinessValidationResult> ValidateBusinessRulesForUpdate(SupplierUpdateDto dto, Supplier entity)
        {
            // Example business rule: Cannot update deleted suppliers
            if (entity.IsDeleted)
            {
                return Task.FromResult(BusinessValidationResult.Failure("Silinmiş tedarikçi güncellenemez."));
            }
            return Task.FromResult(BusinessValidationResult.Success());
        }
    }
}
