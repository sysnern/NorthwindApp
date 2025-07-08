using Microsoft.EntityFrameworkCore;
using NorthwindApp.Data.Context;
using System.Linq.Expressions;

namespace NorthwindApp.Data.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly NorthwindContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(NorthwindContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
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
