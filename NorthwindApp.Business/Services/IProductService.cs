using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;

namespace NorthwindApp.Business.Services
{
    public interface IProductService
    {
        Task<ApiResponse<List<ProductDTO>>> GetAllAsync(ProductFilterDto filter);
        Task<ApiResponse<ProductDTO>> GetByIdAsync(int id);
        Task<ApiResponse<string>> AddAsync(ProductCreateDto dto);
        Task<ApiResponse<string>> UpdateAsync(ProductUpdateDto dto);
        Task<ApiResponse<string>> DeleteAsync(int id);
    }
}
