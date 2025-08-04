using NorthwindApp.Entities.Models;
using NorthwindApp.Data.Repositories;

namespace NorthwindApp.Data.Repositories.Abstract
{
    public interface IProductRepository : IGenericRepository<Product, int>
    {
        Task<List<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<List<Product>> GetProductsBySupplierAsync(int supplierId);
        Task<List<Product>> GetDiscontinuedProductsAsync();
        Task<List<Product>> GetProductsInStockAsync();
    }
}
