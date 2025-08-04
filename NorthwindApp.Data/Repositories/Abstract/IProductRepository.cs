using NorthwindApp.Entities.Models;

namespace NorthwindApp.Data.Repositories.Abstract
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<List<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<List<Product>> GetProductsBySupplierAsync(int supplierId);
        Task<List<Product>> GetDiscontinuedProductsAsync();
        Task<List<Product>> GetProductsInStockAsync();
    }
}
