using Microsoft.EntityFrameworkCore;
using NorthwindApp.Data.Context;
using NorthwindApp.Data.Repositories.Abstract;
using NorthwindApp.Entities.Models;
using NorthwindApp.Data.Repositories;

namespace NorthwindApp.Data.Repositories.Concrete
{
    public class ProductRepository : GenericRepository<Product, int>, IProductRepository
    {
        public ProductRepository(NorthwindContext context) : base(context)
        {
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _dbSet.Where(p => p.CategoryId == categoryId && !p.IsDeleted).ToListAsync();
        }

        public async Task<List<Product>> GetProductsBySupplierAsync(int supplierId)
        {
            return await _dbSet.Where(p => p.SupplierId == supplierId && !p.IsDeleted).ToListAsync();
        }

        public async Task<List<Product>> GetDiscontinuedProductsAsync()
        {
            return await _dbSet.Where(p => p.Discontinued && !p.IsDeleted).ToListAsync();
        }

        public async Task<List<Product>> GetProductsInStockAsync()
        {
            return await _dbSet.Where(p => p.UnitsInStock > 0 && !p.IsDeleted).ToListAsync();
        }
    }
}
