using Microsoft.EntityFrameworkCore;
using NorthwindApp.Data.Context;
using NorthwindApp.Data.Repositories.Abstract;
using NorthwindApp.Entities.Models;

namespace NorthwindApp.Data.Repositories.Concrete
{
    public class SupplierRepository : Repository<Supplier>, ISupplierRepository
    {
        public SupplierRepository(NorthwindContext context) : base(context)
        {
        }

        public async Task<List<Supplier>> GetSuppliersByCountryAsync(string country)
        {
            return await _dbSet.Where(s => s.Country == country && !s.IsDeleted).ToListAsync();
        }

        public async Task<List<Supplier>> GetSuppliersByCityAsync(string city)
        {
            return await _dbSet.Where(s => s.City == city && !s.IsDeleted).ToListAsync();
        }

        public async Task<Supplier?> GetSupplierWithProductsAsync(int supplierId)
        {
            return await _dbSet
                .Include(s => s.Products)
                .FirstOrDefaultAsync(s => s.SupplierId == supplierId && !s.IsDeleted);
        }

        public async Task<List<Supplier>> GetActiveSuppliersAsync()
        {
            return await _dbSet.Where(s => !s.IsDeleted).ToListAsync();
        }
    }
}
