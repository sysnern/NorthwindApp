using NorthwindApp.Entities.Models;
using NorthwindApp.Data.Repositories;

namespace NorthwindApp.Data.Repositories.Abstract
{
    public interface IEmployeeRepository : IGenericRepository<Employee, int>
    {
        Task<List<Employee>> GetEmployeesByTitleAsync(string title);
        Task<List<Employee>> GetEmployeesByCountryAsync(string country);
        Task<Employee?> GetEmployeeWithOrdersAsync(int employeeId);
        Task<List<Employee>> GetActiveEmployeesAsync();
    }
}
