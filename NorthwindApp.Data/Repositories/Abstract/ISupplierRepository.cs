using NorthwindApp.Entities.Models;
using NorthwindApp.Data.Repositories;

namespace NorthwindApp.Data.Repositories.Abstract
{
    public interface ISupplierRepository : IGenericRepository<Supplier, int>
    {
        Task<List<Supplier>> GetSuppliersByCountryAsync(string country);
        Task<List<Supplier>> GetSuppliersByCityAsync(string city);
        Task<Supplier?> GetSupplierWithProductsAsync(int supplierId);
        Task<List<Supplier>> GetActiveSuppliersAsync();
    }
}
