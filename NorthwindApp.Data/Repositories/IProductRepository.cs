using NorthwindApp.Entities.Models;

namespace NorthwindApp.Data.Repositories
{
    public interface IProductRepository : IGenericRepository<Product, int>
    {
    }
}
