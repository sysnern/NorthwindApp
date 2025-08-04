using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;

namespace NorthwindApp.Business.Services.Abstract
{
    public interface ICustomerService
    {
        Task<ApiResponse<List<CustomerDTO>>> GetAllAsync(CustomerFilterDto? filter = null);
        Task<ApiResponse<CustomerDTO>> GetByIdAsync(string id);
        Task<ApiResponse<string>> AddAsync(CustomerCreateDto dto);
        Task<ApiResponse<string>> UpdateAsync(CustomerUpdateDto dto);
        Task<ApiResponse<string>> DeleteAsync(string id);
    }
}
