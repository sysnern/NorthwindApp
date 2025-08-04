using Microsoft.EntityFrameworkCore;
using NorthwindApp.Data.Context;
using NorthwindApp.Data.Repositories.Abstract;
using NorthwindApp.Entities.Models;
using NorthwindApp.Data.Repositories;

namespace NorthwindApp.Data.Repositories.Concrete
{
    public class CustomerRepository : GenericRepository<Customer, string>, ICustomerRepository
    {
        public CustomerRepository(NorthwindContext context) : base(context)
        {
        }

        public async Task<List<Customer>> GetActiveCustomersAsync()
        {
            return await _dbSet.Where(c => !c.IsDeleted).ToListAsync();
        }

        public async Task<Customer?> GetCustomerWithOrdersAsync(string customerId)
        {
            return await _dbSet
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId && !c.IsDeleted);
        }

        public async Task<List<Customer>> GetCustomersByCountryAsync(string country)
        {
            return await _dbSet.Where(c => c.Country == country && !c.IsDeleted).ToListAsync();
        }

        public async Task<List<Customer>> GetCustomersByCityAsync(string city)
        {
            return await _dbSet.Where(c => c.City == city && !c.IsDeleted).ToListAsync();
        }
    }
}
