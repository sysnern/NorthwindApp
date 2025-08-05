using Microsoft.EntityFrameworkCore;
using NorthwindApp.Data.Context;
using NorthwindApp.Data.Repositories.Abstract;
using NorthwindApp.Entities.Models;
using NorthwindApp.Data.Repositories;

namespace NorthwindApp.Data.Repositories.Concrete
{
    public class OrderRepository : GenericRepository<Order, int>, IOrderRepository
    {
        public OrderRepository(NorthwindContext context) : base(context)
        {
        }

        public async Task<List<Order>> GetActiveOrdersAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<Order?> GetOrderWithDetailsAsync(int orderId)
        {
            return await _dbSet
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<List<Order>> GetOrdersByCustomerAsync(string customerId)
        {
            return await _dbSet.Where(o => o.CustomerId == customerId).ToListAsync();
        }

        public async Task<List<Order>> GetOrdersByEmployeeAsync(int employeeId)
        {
            return await _dbSet.Where(o => o.EmployeeId == employeeId).ToListAsync();
        }

        public async Task<List<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .ToListAsync();
        }
    }
}
