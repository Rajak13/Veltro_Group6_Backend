using Microsoft.EntityFrameworkCore;
using Veltro.Data;
using Veltro.Repositories.Interfaces;

namespace Veltro.Repositories;

/// <summary>Generic repository implementation providing standard CRUD operations via EF Core.</summary>
public class BaseRepository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public BaseRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    /// <summary>Returns all entities. Use AsNoTracking for read-only scenarios.</summary>
    public async Task<IEnumerable<T>> GetAllAsync() =>
        await _dbSet.AsNoTracking().ToListAsync();

    /// <summary>Returns a single entity by its Guid primary key.</summary>
    public async Task<T?> GetByIdAsync(Guid id) =>
        await _dbSet.FindAsync(id);

    /// <summary>Adds a new entity to the context (not yet saved).</summary>
    public async Task AddAsync(T entity) =>
        await _dbSet.AddAsync(entity);

    /// <summary>Marks an entity as modified.</summary>
    public void Update(T entity) =>
        _dbSet.Update(entity);

    /// <summary>Marks an entity for deletion.</summary>
    public void Delete(T entity) =>
        _dbSet.Remove(entity);

    /// <summary>Persists all pending changes to the database.</summary>
    public async Task<bool> SaveChangesAsync() =>
        await _context.SaveChangesAsync() > 0;
}
