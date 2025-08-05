using Microsoft.EntityFrameworkCore;
using NorthwindApp.Data.Context;
using NorthwindApp.Data.Repositories.Abstract;
using System.Linq.Expressions;
using System.Reflection;

namespace NorthwindApp.Data.Repositories.Concrete
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly NorthwindContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(NorthwindContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null)
        {
            if (filter == null)
                return await _dbSet.ToListAsync();
            
            return await _dbSet.Where(filter).ToListAsync();
        }

        public async Task<List<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            string? sortField = null,
            string? sortDirection = null,
            int page = 1,
            int pageSize = 10)
        {
            IQueryable<T> query = _dbSet;
            
            // Apply filter
            if (filter != null)
                query = query.Where(filter);

            // Apply sorting
            if (!string.IsNullOrEmpty(sortField))
            {
                var property = typeof(T).GetProperty(sortField, 
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                
                if (property != null)
                {
                    var parameter = Expression.Parameter(typeof(T), "x");
                    var propertyAccess = Expression.Property(parameter, property);
                    var lambda = Expression.Lambda(propertyAccess, parameter);
                    
                    var methodName = sortDirection?.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";
                    var method = typeof(Queryable).GetMethods()
                        .Where(m => m.Name == methodName && m.GetParameters().Length == 2)
                        .First()
                        .MakeGenericMethod(typeof(T), property.PropertyType);
                    
                    query = (IQueryable<T>)method.Invoke(null, new object[] { query, lambda })!;
                }
            }

            // Apply pagination
            if (page > 0 && pageSize > 0)
            {
                query = query.Skip((page - 1) * pageSize).Take(pageSize);
            }

            return await query.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T?> GetByIdAsync(string id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
