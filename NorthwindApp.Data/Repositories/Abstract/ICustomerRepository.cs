using NorthwindApp.Entities.Models;

namespace NorthwindApp.Data.Repositories.Abstract
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<List<Customer>> GetCustomersByCountryAsync(string country);
        Task<List<Customer>> GetCustomersByCityAsync(string city);
        Task<Customer?> GetCustomerWithOrdersAsync(string customerId);
        Task<List<Customer>> GetActiveCustomersAsync();
    }
}
