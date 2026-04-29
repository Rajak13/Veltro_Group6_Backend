using Veltro.Models;

namespace Veltro.Data;

/// <summary>Seeds the database with default users for testing.</summary>
public static class Seeder
{
    /// <summary>Creates default Admin, Staff, and Customer accounts if they don't exist.</summary>
    public static async Task SeedAsync(AppDbContext context)
    {
        // Check if seeding has already been done
        if (context.Users.Any(u => u.Role == UserRole.Admin))
            return;

        // Create Admin user
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

        // Create Staff user
        var staffUserId = Guid.NewGuid();
        var staffUser = new User
        {
            Id = staffUserId,
            FullName = "Test Staff",
            Email = "staff@gmail.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test123@"),
            Role = UserRole.Staff,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        context.Users.Add(staffUser);

        var staff = new Staff
        {
            UserId = staffUserId,
            Position = "Service Technician"
        };
        context.Staff.Add(staff);

        // Create Customer user
        var customerUserId = Guid.NewGuid();
        var customerUser = new User
        {
            Id = customerUserId,
            FullName = "Test Customer",
            Email = "customer1@gmail.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test123@"),
            Role = UserRole.Customer,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        context.Users.Add(customerUser);

        var customer = new Customer
        {
            CustomerId = Guid.NewGuid(),
            UserId = customerUserId,
            Phone = "1234567890",
            Address = "123 Test Street"
        };
        context.Customers.Add(customer);

        await context.SaveChangesAsync();
    }
}
