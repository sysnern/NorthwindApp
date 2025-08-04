using Microsoft.EntityFrameworkCore;
using NorthwindApp.Data.Context;
using NorthwindApp.Data.Repositories.Abstract;
using NorthwindApp.Entities.Models;

namespace NorthwindApp.Data.Repositories.Concrete
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(NorthwindContext context) : base(context)
        {
        }

        public async Task<List<Order>> GetOrdersByCustomerAsync(string customerId)
        {
            return await _dbSet.Where(o => o.CustomerId == customerId && !o.IsDeleted).ToListAsync();
        }

        public async Task<List<Order>> GetOrdersByEmployeeAsync(int employeeId)
        {
            return await _dbSet.Where(o => o.EmployeeId == employeeId && !o.IsDeleted).ToListAsync();
        }

        public async Task<List<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate && !o.IsDeleted)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderWithDetailsAsync(int orderId)
        {
            return await _dbSet
                .Include(o => o.OrderDetails)
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && !o.IsDeleted);
        }

        public async Task<List<Order>> GetActiveOrdersAsync()
        {
            return await _dbSet.Where(o => !o.IsDeleted).ToListAsync();
        }
    }
}
