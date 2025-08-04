using Microsoft.EntityFrameworkCore;
using NorthwindApp.Data.Context;
using NorthwindApp.Data.Repositories.Abstract;
using System.Linq.Expressions;

namespace NorthwindApp.Data.Repositories
{
    /// <summary>
    /// Generic repository implementation that provides common data access operations
    /// Eliminates code duplication across repository implementations
    /// </summary>
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>, IRepository<TEntity> 
        where TEntity : class
    {
        protected readonly NorthwindContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public GenericRepository(NorthwindContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public virtual async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null)
        {
            IQueryable<TEntity> query = _dbSet;
            
            if (filter != null)
                query = query.Where(filter);

            return await query.ToListAsync();
        }

        public virtual async Task<TEntity?> GetByIdAsync(TKey id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public virtual async Task<bool> ExistsAsync(TKey id)
        {
            var entity = await GetByIdAsync(id);
            return entity != null;
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null)
        {
            IQueryable<TEntity> query = _dbSet;
            
            if (filter != null)
                query = query.Where(filter);

            return await query.CountAsync();
        }

        // IRepository<T> interface methods
        public async Task<List<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(int id)
        {
            // This is a fallback for int-based IDs
            if (typeof(TKey) == typeof(int))
            {
                return await GetByIdAsync((TKey)(object)id);
            }
            throw new NotSupportedException($"GetByIdAsync(int) is not supported for {typeof(TKey)} key type");
        }

        public async Task<TEntity?> GetByIdAsync(string id)
        {
            // This is a fallback for string-based IDs
            if (typeof(TKey) == typeof(string))
            {
                return await GetByIdAsync((TKey)(object)id);
            }
            throw new NotSupportedException($"GetByIdAsync(string) is not supported for {typeof(TKey)} key type");
        }
    }
}