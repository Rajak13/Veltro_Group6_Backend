using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Veltro.Models;

/// <summary>In-app notification record for a User.</summary>
public class Notification
{
    [Key]
    public Guid NotificationId { get; set; } = Guid.NewGuid();

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    [Required]
    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public NotificationType Type { get; set; } = NotificationType.General;

    // Navigation
    public User User { get; set; } = null!;
}
