using NorthwindApp.Entities.Models;

namespace NorthwindApp.Data.Repositories.Abstract
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<List<Employee>> GetEmployeesByTitleAsync(string title);
        Task<List<Employee>> GetEmployeesByCountryAsync(string country);
        Task<Employee?> GetEmployeeWithOrdersAsync(int employeeId);
        Task<List<Employee>> GetActiveEmployeesAsync();
    }
}
