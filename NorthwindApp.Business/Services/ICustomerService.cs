using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;

namespace NorthwindApp.Business.Services
{
    public interface ICustomerService
    {
        Task<ApiResponse<List<CustomerDTO>>> GetAllAsync();
        Task<ApiResponse<CustomerDTO>> GetByIdAsync(string id);
        Task<ApiResponse<string>> AddAsync(CustomerCreateDto dto);
        Task<ApiResponse<string>> UpdateAsync(CustomerUpdateDto dto);
        Task<ApiResponse<string>> DeleteAsync(string id);
    }
}
