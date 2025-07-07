using NorthwindApp.Core.DTOs;

namespace NorthwindApp.Business.Services
{
    public interface IProductService
    {
        Task<List<ProductDTO>> GetAllAsync(ProductFilterDto filter);
        Task<ProductDTO?> GetByIdAsync(int id);
        Task AddAsync(ProductCreateDto dto);
        Task UpdateAsync(ProductUpdateDto dto);
        Task DeleteAsync(int id);
    }
}
