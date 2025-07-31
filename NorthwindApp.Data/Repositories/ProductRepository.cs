using NorthwindApp.Data.Context;
using NorthwindApp.Entities.Models;

namespace NorthwindApp.Data.Repositories
{
    public class ProductRepository : GenericRepository<Product, int>, IProductRepository
    {

        public ProductRepository(NorthwindContext context)
            : base(context)
        {
        }
    }
}
