namespace Veltro.Repositories.Interfaces;

/// <summary>Generic base repository interface providing standard CRUD operations.</summary>
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<bool> SaveChangesAsync();
}
