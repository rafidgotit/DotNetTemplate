using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Implementation;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
{
    private readonly PortalDbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public BaseRepository(PortalDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public IQueryable<TEntity?> GetAll()
    {
        return _dbSet;
    }

    public async Task<TEntity?> GetById(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public void Add(TEntity? entity)
    {
        if (entity != null) _dbSet.Add(entity);
    }

    public void Update(TEntity? entity)
    {
        if (entity != null) _dbSet.Update(entity);
    }

    public void Delete(TEntity? entity)
    {
        if (entity != null) _dbSet.Remove(entity);
    }
    
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}