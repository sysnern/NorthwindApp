using System.Linq.Expressions;
using NorthwindApp.Data.Repositories.Abstract;

namespace NorthwindApp.Data.Repositories
{
    /// <summary>
    /// Generic repository interface that standardizes data access patterns
    /// Eliminates inconsistencies between different repository implementations
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TKey">Primary key type</typeparam>
    public interface IGenericRepository<TEntity, TKey> : IRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(TKey id);
        Task<bool> ExistsAsync(TKey id);
        Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null);
    }
}