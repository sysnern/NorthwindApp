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
    public class EmployeeService : GenericService<Employee, EmployeeDTO, EmployeeCreateDto, EmployeeUpdateDto, int>, IEmployeeService
    {
        public EmployeeService(
            IEmployeeRepository employeeRepo,
            IMapper mapper,
            ICacheService cacheService,
            ILogger<EmployeeService> logger,
            IHttpContextAccessor httpContextAccessor)
            : base(employeeRepo, mapper, cacheService, logger, httpContextAccessor, "employee_list_", "Çalışan")
        {
        }

        // Override to handle EmployeeFilterDto specifically with pagination and sorting
        public async Task<ApiResponse<List<EmployeeDTO>>> GetAllAsync(EmployeeFilterDto? filter = null)
        {
            // Build filter expression
            Expression<Func<Employee, bool>>? filterExpression = null;
            
            if (filter != null)
            {
                filterExpression = e =>
                (string.IsNullOrEmpty(filter.LastName) || (e.LastName != null && e.LastName.ToLower().Contains(filter.LastName.ToLower()))) &&
                (string.IsNullOrEmpty(filter.FirstName) || (e.FirstName != null && e.FirstName.ToLower().Contains(filter.FirstName.ToLower()))) &&
                (string.IsNullOrEmpty(filter.Title) || (e.Title != null && e.Title.ToLower().Contains(filter.Title.ToLower()))) &&
                (string.IsNullOrEmpty(filter.City) || (e.City != null && e.City.ToLower().Contains(filter.City.ToLower()))) &&
                (string.IsNullOrEmpty(filter.Country) || (e.Country != null && e.Country.ToLower().Contains(filter.Country.ToLower()))) &&
                (!filter.IsDeleted.HasValue || e.IsDeleted == filter.IsDeleted);
            }

            // Extract pagination and sorting parameters from filter
            var sortField = filter?.SortField ?? "EmployeeId";
            var sortDirection = filter?.SortDirection ?? "asc";
            var page = filter?.Page ?? 1;
            var pageSize = filter?.PageSize ?? 10;

            // Generate cache key with filter parameters
            var cacheKey = GenerateCacheKeyWithFilter(filter);

            return await base.GetAllAsync(filterExpression, sortField, sortDirection, page, pageSize, cacheKey);
        }

        private string GenerateCacheKeyWithFilter(EmployeeFilterDto? filter)
        {
            if (filter == null)
                return _cachePrefix;

            // Create a unique cache key based on filter parameters
            var filterParams = new List<string>
            {
                $"ln:{filter.LastName ?? ""}",
                $"fn:{filter.FirstName ?? ""}",
                $"title:{filter.Title ?? ""}",
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

        protected override int GetIdFromUpdateDto(EmployeeUpdateDto dto)
        {
            return dto.EmployeeId;
        }

        protected override bool SupportsSoftDelete() => true;

        protected override void PerformSoftDelete(Employee entity)
        {
            entity.IsDeleted = true;
        }



        // Implement IEmployeeService methods that return string instead of EmployeeDTO
        async Task<ApiResponse<string>> IEmployeeService.AddAsync(EmployeeCreateDto dto)
        {
            var result = await base.AddAsync(dto);
            if (result.Success)
            {
                return ApiResponse<string>.Created(null, result.Message);
            }
            return ApiResponse<string>.BadRequest(result.Errors ?? new[] { result.Message }, result.Message);
        }

        async Task<ApiResponse<string>> IEmployeeService.UpdateAsync(EmployeeUpdateDto dto)
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
        protected override Task<BusinessValidationResult> ValidateBusinessRulesForCreate(EmployeeCreateDto dto)
        {
            // Example business rule: Employee must have a valid title
            if (string.IsNullOrEmpty(dto.Title))
            {
                return Task.FromResult(BusinessValidationResult.Failure("Çalışan pozisyonu boş olamaz."));
            }
            return Task.FromResult(BusinessValidationResult.Success());
        }

        protected override Task<BusinessValidationResult> ValidateBusinessRulesForUpdate(EmployeeUpdateDto dto, Employee entity)
        {
            // Example business rule: Cannot update deleted employees
            if (entity.IsDeleted)
            {
                return Task.FromResult(BusinessValidationResult.Failure("Silinmiş çalışan güncellenemez."));
            }
            return Task.FromResult(BusinessValidationResult.Success());
        }
    }
}
