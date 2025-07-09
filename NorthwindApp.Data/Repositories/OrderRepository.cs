using Microsoft.EntityFrameworkCore;
using NorthwindApp.Data.Context;
using NorthwindApp.Entities.Models;
using System.Linq.Expressions;

namespace NorthwindApp.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly NorthwindContext _context;

        public OrderRepository(NorthwindContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllAsync(Expression<Func<Order, bool>>? filter = null)
        {
            return filter is null
                ? await _context.Orders.ToListAsync()
                : await _context.Orders.Where(filter).ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int id) =>
            await _context.Orders.FindAsync(id);

        public async Task AddAsync(Order order) =>
            await _context.Orders.AddAsync(order);

        public void Update(Order order) =>
            _context.Orders.Update(order);

        public void Delete(Order order) =>
            _context.Orders.Remove(order);

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }
}
