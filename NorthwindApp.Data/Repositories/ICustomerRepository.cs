using NorthwindApp.Entities.Models;
using System.Linq.Expressions;

namespace NorthwindApp.Data.Repositories
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetAllAsync(Expression<Func<Customer, bool>> filter = null!);
        Task<Customer?> GetByIdAsync(string id);
        Task AddAsync(Customer customer);
        void Update(Customer customer);
        void Delete(Customer customer);
        Task SaveChangesAsync();
    }
}
