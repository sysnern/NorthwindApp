using Microsoft.EntityFrameworkCore;
using NorthwindApp.Data.Context;
using NorthwindApp.Entities.Models;
using System.Linq.Expressions;

namespace NorthwindApp.Data.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly NorthwindContext _context;

        public EmployeeRepository(NorthwindContext context)
        {
            _context = context;
        }

        public async Task<List<Employee>> GetAllAsync(Expression<Func<Employee, bool>> filter = null!)
        {
            return filter is null
                ? await _context.Employees.ToListAsync()
                : await _context.Employees.Where(filter).ToListAsync();
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees.FindAsync(id);
        }

        public async Task AddAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
        }

        public void Update(Employee employee)
        {
            _context.Employees.Update(employee);
        }

        public void Delete(Employee employee)
        {
            _context.Employees.Remove(employee);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
