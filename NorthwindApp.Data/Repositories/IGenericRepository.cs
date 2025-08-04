using System.Linq.Expressions;

namespace NorthwindApp.Data.Repositories
{
    /// <summary>
    /// Generic repository interface that standardizes data access patterns
    /// Eliminates inconsistencies between different repository implementations
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TKey">Primary key type</typeparam>
    public interface IGenericRepository<TEntity, TKey> where TEntity : class
    {
        Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null);
        Task<TEntity?> GetByIdAsync(TKey id);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task SaveChangesAsync();
        Task<bool> ExistsAsync(TKey id);
        Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null);
    }
}