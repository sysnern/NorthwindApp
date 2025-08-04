using NorthwindApp.Entities.Models;

namespace NorthwindApp.Data.Repositories.Abstract
{
    public interface ISupplierRepository : IRepository<Supplier>
    {
        Task<List<Supplier>> GetSuppliersByCountryAsync(string country);
        Task<List<Supplier>> GetSuppliersByCityAsync(string city);
        Task<Supplier?> GetSupplierWithProductsAsync(int supplierId);
        Task<List<Supplier>> GetActiveSuppliersAsync();
    }
}
