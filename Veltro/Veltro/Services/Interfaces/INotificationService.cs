using Veltro.DTOs.Response.Notification;
using Veltro.Models;

namespace Veltro.Services.Interfaces;

/// <summary>In-app notification service contract.</summary>
public interface INotificationService
{
    Task CreateNotificationAsync(Guid userId, string message, NotificationType type);
    Task<IEnumerable<NotificationResponseDto>> GetUserNotificationsAsync(Guid userId);
    Task MarkAsReadAsync(Guid notificationId);
}
