using AutoMapper;
using NorthwindApp.Business.Services.Abstract;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;
using NorthwindApp.Data.Repositories.Abstract;
using NorthwindApp.Entities.Models;
using System.Linq.Expressions;

namespace NorthwindApp.Business.Services.Concrete
{
    public class EmployeeService : GenericService<Employee, EmployeeDTO, EmployeeCreateDto, EmployeeUpdateDto, int>, IEmployeeService
    {
        public EmployeeService(
            IEmployeeRepository repo,
            IMapper mapper,
            ICacheService cacheService)
            : base(repo, mapper, cacheService, "employee_list_", "Çalışan")
        {
        }

        protected override int GetIdFromUpdateDto(EmployeeUpdateDto dto)
        {
            return dto.EmployeeId;
        }

        // Override to handle EmployeeFilterDto specifically
        public async Task<ApiResponse<List<EmployeeDTO>>> GetAllAsync(EmployeeFilterDto? filter = null)
        {
            // Build filter expression
            Expression<Func<Employee, bool>>? filterExpression = null;
            
            if (filter != null)
            {
                filterExpression = e =>
                    (string.IsNullOrEmpty(filter.FirstName) || (e.FirstName != null && e.FirstName.Contains(filter.FirstName))) &&
                    (string.IsNullOrEmpty(filter.LastName) || (e.LastName != null && e.LastName.Contains(filter.LastName))) &&
                    (string.IsNullOrEmpty(filter.Title) || (e.Title != null && e.Title.Contains(filter.Title))) &&
                    (string.IsNullOrEmpty(filter.Country) || (e.Country != null && e.Country.Contains(filter.Country))) &&
                    (!filter.IsDeleted.HasValue || e.IsDeleted == filter.IsDeleted);
            }

            return await base.GetAllAsync(filterExpression);
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
        protected override async Task<BusinessValidationResult> ValidateBusinessRulesForCreate(EmployeeCreateDto dto)
        {
            // Example business rule: Employee must have a valid title
            if (string.IsNullOrEmpty(dto.Title))
            {
                return BusinessValidationResult.Failure("Çalışan pozisyonu boş olamaz.");
            }
            return BusinessValidationResult.Success();
        }

        protected override async Task<BusinessValidationResult> ValidateBusinessRulesForUpdate(EmployeeUpdateDto dto, Employee entity)
        {
            // Example business rule: Cannot update deleted employees
            if (entity.IsDeleted)
            {
                return BusinessValidationResult.Failure("Silinmiş çalışan güncellenemez.");
            }
            return BusinessValidationResult.Success();
        }
    }
}
