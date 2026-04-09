using System.ComponentModel.DataAnnotations;

namespace Veltro.Models;

/// <summary>Application user — base identity for Admin, Staff, and Customer roles.</summary>
public class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    // Navigation
    public Customer? Customer { get; set; }
    public Staff? Staff { get; set; }
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
