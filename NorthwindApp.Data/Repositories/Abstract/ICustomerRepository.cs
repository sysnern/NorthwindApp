using NorthwindApp.Entities.Models;
using NorthwindApp.Data.Repositories;

namespace NorthwindApp.Data.Repositories.Abstract
{
    public interface ICustomerRepository : IGenericRepository<Customer, string>
    {
        Task<List<Customer>> GetCustomersByCountryAsync(string country);
        Task<List<Customer>> GetCustomersByCityAsync(string city);
        Task<Customer?> GetCustomerWithOrdersAsync(string customerId);
        Task<List<Customer>> GetActiveCustomersAsync();
    }
}
