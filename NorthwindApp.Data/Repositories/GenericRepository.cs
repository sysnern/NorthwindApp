using Microsoft.EntityFrameworkCore;
using NorthwindApp.Data.Context;
using NorthwindApp.Data.Repositories.Abstract;
using System.Linq.Expressions;
using System.Reflection;

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

        public virtual async Task<List<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            string? sortField = null,
            string? sortDirection = null,
            int page = 1,
            int pageSize = 10)
        {
            IQueryable<TEntity> query = _dbSet;
            
            // Apply filter
            if (filter != null)
                query = query.Where(filter);

            // Apply sorting
            if (!string.IsNullOrEmpty(sortField))
            {
                var property = typeof(TEntity).GetProperty(sortField, 
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                
                if (property != null)
                {
                    var parameter = Expression.Parameter(typeof(TEntity), "x");
                    var propertyAccess = Expression.Property(parameter, property);
                    var lambda = Expression.Lambda(propertyAccess, parameter);
                    
                    var methodName = sortDirection?.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";
                    var method = typeof(Queryable).GetMethods()
                        .Where(m => m.Name == methodName && m.GetParameters().Length == 2)
                        .First()
                        .MakeGenericMethod(typeof(TEntity), property.PropertyType);
                    
                    query = (IQueryable<TEntity>)method.Invoke(null, new object[] { query, lambda })!;
                }
                else
                {
                    // Log or handle case where property is not found
                    // For now, we'll just skip sorting if property is not found
                }
            }
            else
            {
                // Default sorting to avoid EF warning about Skip/Take without OrderBy
                // Try to find an ID property for default sorting
                var idProperty = typeof(TEntity).GetProperty("Id") ?? 
                                typeof(TEntity).GetProperty("ProductId") ?? 
                                typeof(TEntity).GetProperty("CategoryId") ?? 
                                typeof(TEntity).GetProperty("SupplierId") ?? 
                                typeof(TEntity).GetProperty("EmployeeId") ?? 
                                typeof(TEntity).GetProperty("OrderId") ?? 
                                typeof(TEntity).GetProperty("CustomerId");
                
                if (idProperty != null)
                {
                    var parameter = Expression.Parameter(typeof(TEntity), "x");
                    var propertyAccess = Expression.Property(parameter, idProperty);
                    var lambda = Expression.Lambda(propertyAccess, parameter);
                    
                    var method = typeof(Queryable).GetMethods()
                        .Where(m => m.Name == "OrderBy" && m.GetParameters().Length == 2)
                        .First()
                        .MakeGenericMethod(typeof(TEntity), idProperty.PropertyType);
                    
                    query = (IQueryable<TEntity>)method.Invoke(null, new object[] { query, lambda })!;
                }
            }

            // Apply pagination
            if (page > 0 && pageSize > 0)
            {
                query = query.Skip((page - 1) * pageSize).Take(pageSize);
            }

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

        // IRepository<T> interface methods - these are now handled by the generic methods above
        public async Task<List<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        // These methods are now handled by the generic GetByIdAsync(TKey id) method
        // and will be called through the interface implementation
        async Task<TEntity?> IRepository<TEntity>.GetByIdAsync(int id)
        {
            if (typeof(TKey) == typeof(int))
            {
                return await GetByIdAsync((TKey)(object)id);
            }
            throw new NotSupportedException($"GetByIdAsync(int) is not supported for {typeof(TKey)} key type");
        }

        async Task<TEntity?> IRepository<TEntity>.GetByIdAsync(string id)
        {
            if (typeof(TKey) == typeof(string))
            {
                return await GetByIdAsync((TKey)(object)id);
            }
            throw new NotSupportedException($"GetByIdAsync(string) is not supported for {typeof(TKey)} key type");
        }
    }
}