using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;

namespace NorthwindApp.Business.Services
{
    public interface IEmployeeService
    {
        Task<ApiResponse<List<EmployeeDTO>>> GetAllAsync();
        Task<ApiResponse<EmployeeDTO>> GetByIdAsync(int id);
        Task<ApiResponse<string>> AddAsync(EmployeeCreateDto dto);
        Task<ApiResponse<string>> UpdateAsync(EmployeeUpdateDto dto);
        Task<ApiResponse<string>> DeleteAsync(int id);
    }
}
