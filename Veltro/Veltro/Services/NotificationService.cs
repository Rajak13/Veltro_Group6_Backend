using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Veltro.Data;
using Veltro.DTOs.Response.Notification;
using Veltro.Models;
using Veltro.Services.Interfaces;

namespace Veltro.Services;

/// <summary>Creates and retrieves in-app notifications for users.</summary>
public class NotificationService : INotificationService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(AppDbContext context, IMapper mapper, ILogger<NotificationService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>Creates a new notification record for the specified user.</summary>
    public async Task CreateNotificationAsync(Guid userId, string message, NotificationType type)
    {
        try
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                Type = type
            };
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create notification for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>Returns all notifications for a user, newest first.</summary>
    public async Task<IEnumerable<NotificationResponseDto>> GetUserNotificationsAsync(Guid userId)
    {
        try
        {
            return await _context.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationResponseDto
                {
                    NotificationId = n.NotificationId,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt,
                    Type = n.Type.ToString()
                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get notifications for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>Marks a notification as read.</summary>
    public async Task MarkAsReadAsync(Guid notificationId)
    {
        try
        {
            var notification = await _context.Notifications.FindAsync(notificationId)
                ?? throw new KeyNotFoundException("Notification not found.");
            notification.IsRead = true;
            await _context.SaveChangesAsync();
        }
        catch (Exception ex) when (ex is not KeyNotFoundException)
        {
            _logger.LogError(ex, "Failed to mark notification {Id} as read", notificationId);
            throw;
        }
    }
}
