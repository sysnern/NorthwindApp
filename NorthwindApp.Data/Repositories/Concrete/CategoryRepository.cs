using Microsoft.EntityFrameworkCore;
using NorthwindApp.Data.Context;
using NorthwindApp.Data.Repositories.Abstract;
using NorthwindApp.Entities.Models;
using NorthwindApp.Data.Repositories;

namespace NorthwindApp.Data.Repositories.Concrete
{
    public class CategoryRepository : GenericRepository<Category, int>, ICategoryRepository
    {
        public CategoryRepository(NorthwindContext context) : base(context)
        {
        }

        public async Task<List<Category>> GetActiveCategoriesAsync()
        {
            return await _dbSet.Where(c => !c.IsDeleted).ToListAsync();
        }

        public async Task<Category?> GetCategoryWithProductsAsync(int categoryId)
        {
            return await _dbSet
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId && !c.IsDeleted);
        }
    }
}
