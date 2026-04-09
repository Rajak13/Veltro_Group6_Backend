using Microsoft.EntityFrameworkCore;
using Veltro.Data;
using Veltro.Models;
using Veltro.Repositories.Interfaces;

namespace Veltro.Repositories;

/// <summary>User repository with email and role lookup support.</summary>
public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    /// <summary>Finds a user by their unique email address.</summary>
    public async Task<User?> GetByEmailAsync(string email) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Email == email.ToLower());

    /// <summary>Returns all users with a specific role.</summary>
    public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role) =>
        await _context.Users
            .AsNoTracking()
            .Where(u => u.Role == role)
            .ToListAsync();
}
