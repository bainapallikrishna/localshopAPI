using LocalShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LocalShop.Infrastructure.Repositories
{
    public abstract class RepositoryBase<TEntity> where TEntity : class
    {
        protected readonly LocalShopDbContext _context;
        protected readonly DbSet<TEntity> _set;

        protected RepositoryBase(LocalShopDbContext context)
        {
            _context = context;
            _set = _context.Set<TEntity>();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _set.ToListAsync();
        }

        public virtual async Task<TEntity?> GetByIdAsync(int id)
        {
            return await _set.FindAsync(id);
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            await _set.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            _set.Update(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(TEntity entity)
        {
            _set.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}


