namespace Veltro.DTOs.Response.Notification;

/// <summary>Notification details returned in responses.</summary>
public class NotificationResponseDto
{
    public Guid NotificationId { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Type { get; set; } = string.Empty;
}
