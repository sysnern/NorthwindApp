using NorthwindApp.Entities.Models;
using System.Linq.Expressions;

namespace NorthwindApp.Data.Repositories
{
    public interface ISupplierRepository
    {
        Task<List<Supplier>> GetAllAsync(Expression<Func<Supplier, bool>> filter = null!);
        Task<Supplier?> GetByIdAsync(int id);
        Task AddAsync(Supplier supplier);
        void Update(Supplier supplier);
        void Delete(Supplier supplier);
        Task SaveChangesAsync();
    }
}
