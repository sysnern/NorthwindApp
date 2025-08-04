using NorthwindApp.Entities.Models;

namespace NorthwindApp.Data.Repositories.Abstract
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<List<Category>> GetActiveCategoriesAsync();
        Task<Category?> GetCategoryWithProductsAsync(int categoryId);
    }
}
