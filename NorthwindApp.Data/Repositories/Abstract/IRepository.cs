using System.Linq.Expressions;

namespace NorthwindApp.Data.Repositories.Abstract
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);
        Task<List<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            string? sortField = null,
            string? sortDirection = null,
            int page = 1,
            int pageSize = 10);
        Task<T?> GetByIdAsync(int id);
        Task<T?> GetByIdAsync(string id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task SaveChangesAsync();
    }
}
