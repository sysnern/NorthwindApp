using NorthwindApp.Entities.Models;
using NorthwindApp.Data.Repositories;

namespace NorthwindApp.Data.Repositories.Abstract
{
    public interface ICategoryRepository : IGenericRepository<Category, int>
    {
        Task<List<Category>> GetActiveCategoriesAsync();
        Task<Category?> GetCategoryWithProductsAsync(int categoryId);
    }
}
