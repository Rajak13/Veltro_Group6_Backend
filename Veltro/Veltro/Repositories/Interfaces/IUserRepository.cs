using Veltro.Models;

namespace Veltro.Repositories.Interfaces;

/// <summary>User-specific repository operations.</summary>
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetByRoleAsync(UserRole role);
}
