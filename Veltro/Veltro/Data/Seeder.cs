using Veltro.Models;

namespace Veltro.Data;

/// <summary>Seeds the database with a default Admin user on first run.</summary>
public static class Seeder
{
    /// <summary>Creates the default Admin account if none exists.</summary>
    public static async Task SeedAsync(AppDbContext context)
    {
        if (context.Users.Any(u => u.Role == UserRole.Admin))
            return;

        var admin = new User
        {
            Id = Guid.NewGuid(),
            FullName = "System Admin",
            Email = "admin@veltro.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@1234"),
            Role = UserRole.Admin,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Users.Add(admin);
        await context.SaveChangesAsync();
    }
}
