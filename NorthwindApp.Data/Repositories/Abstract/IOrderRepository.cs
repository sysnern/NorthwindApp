using NorthwindApp.Entities.Models;
using NorthwindApp.Data.Repositories;

namespace NorthwindApp.Data.Repositories.Abstract
{
    public interface IOrderRepository : IGenericRepository<Order, int>
    {
        Task<List<Order>> GetOrdersByCustomerAsync(string customerId);
        Task<List<Order>> GetOrdersByEmployeeAsync(int employeeId);
        Task<List<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<Order?> GetOrderWithDetailsAsync(int orderId);
        Task<List<Order>> GetActiveOrdersAsync();
    }
}
