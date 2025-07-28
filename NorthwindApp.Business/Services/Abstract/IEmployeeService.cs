using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;

namespace NorthwindApp.Business.Services.Abstract
{
    public interface IEmployeeService
    {
        Task<ApiResponse<List<EmployeeDTO>>> GetAllAsync();
        Task<ApiResponse<EmployeeDTO>> GetByIdAsync(int id);
        Task<ApiResponse<EmployeeDTO>> AddAsync(EmployeeCreateDto dto);
        Task<ApiResponse<EmployeeDTO>> UpdateAsync(EmployeeUpdateDto dto);
        Task<ApiResponse<string>> DeleteAsync(int id);
    }
}
