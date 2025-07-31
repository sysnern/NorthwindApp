using NorthwindApp.Data.Context;
using NorthwindApp.Entities.Models;

namespace NorthwindApp.Data.Repositories
{
    public class CategoryRepository : GenericRepository<Category, int>, ICategoryRepository
    {
        public CategoryRepository(NorthwindContext context) : base(context)
        {
        }
    }
}
