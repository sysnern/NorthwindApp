using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;

namespace NorthwindApp.Business.Services
{
    public interface IOrderService
    {
        Task<ApiResponse<List<OrderDTO>>> GetAllAsync();
        Task<ApiResponse<OrderDTO>> GetByIdAsync(int id);
        Task<ApiResponse<string>> AddAsync(OrderCreateDto dto);
        Task<ApiResponse<string>> UpdateAsync(OrderUpdateDto dto);
        Task<ApiResponse<string>> DeleteAsync(int id);
    }
}
