using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;

namespace NorthwindApp.Business.Services.Abstract
{
    public interface ICustomerService
    {
        Task<ApiResponse<List<CustomerDTO>>> GetAllAsync();
        Task<ApiResponse<CustomerDTO>> GetByIdAsync(string id);
        Task<ApiResponse<CustomerDTO>> AddAsync(CustomerCreateDto dto);
        Task<ApiResponse<CustomerDTO>> UpdateAsync(CustomerUpdateDto dto);
        Task<ApiResponse<string>> DeleteAsync(string id);
    }
}
