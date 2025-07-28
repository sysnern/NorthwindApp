using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;

namespace NorthwindApp.Business.Services.Abstract
{
    public interface IOrderService
    {
        Task<ApiResponse<List<OrderDTO>>> GetAllAsync();
        Task<ApiResponse<OrderDTO>> GetByIdAsync(int id);
        Task<ApiResponse<OrderDTO>> AddAsync(OrderCreateDto dto);
        Task<ApiResponse<OrderDTO>> UpdateAsync(OrderUpdateDto dto);
        Task<ApiResponse<string>> DeleteAsync(int id);
    }
}
