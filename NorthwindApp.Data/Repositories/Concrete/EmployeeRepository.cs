using Microsoft.EntityFrameworkCore;
using NorthwindApp.Data.Context;
using NorthwindApp.Data.Repositories.Abstract;
using NorthwindApp.Entities.Models;
using NorthwindApp.Data.Repositories;

namespace NorthwindApp.Data.Repositories.Concrete
{
    public class EmployeeRepository : GenericRepository<Employee, int>, IEmployeeRepository
    {
        public EmployeeRepository(NorthwindContext context) : base(context)
        {
        }

        public async Task<List<Employee>> GetActiveEmployeesAsync()
        {
            return await _dbSet.Where(e => !e.IsDeleted).ToListAsync();
        }

        public async Task<Employee?> GetEmployeeWithOrdersAsync(int employeeId)
        {
            return await _dbSet
                .Include(e => e.Orders)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId && !e.IsDeleted);
        }

        public async Task<List<Employee>> GetEmployeesByTitleAsync(string title)
        {
            return await _dbSet.Where(e => e.Title == title && !e.IsDeleted).ToListAsync();
        }

        public async Task<List<Employee>> GetEmployeesByCountryAsync(string country)
        {
            return await _dbSet.Where(e => e.Country == country && !e.IsDeleted).ToListAsync();
        }
    }
}
