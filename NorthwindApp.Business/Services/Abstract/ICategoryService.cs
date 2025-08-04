using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;

namespace NorthwindApp.Business.Services.Abstract
{
    public interface ICategoryService
    {
        Task<ApiResponse<List<CategoryDTO>>> GetAllAsync(CategoryFilterDto? filter = null);
        Task<ApiResponse<CategoryDTO>> GetByIdAsync(int id);
        Task<ApiResponse<string>> AddAsync(CategoryCreateDto dto);
        Task<ApiResponse<string>> UpdateAsync(CategoryUpdateDto dto);
        Task<ApiResponse<string>> DeleteAsync(int id);
    }
}
