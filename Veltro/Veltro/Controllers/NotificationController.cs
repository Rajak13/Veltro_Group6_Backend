using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veltro.Helpers;
using Veltro.Services.Interfaces;

namespace Veltro.Controllers;

/// <summary>In-app notification endpoints for authenticated users.</summary>
[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Returns all notifications for the authenticated user.</summary>
    [HttpGet]
    public async Task<IActionResult> GetMyNotifications()
    {
        var notifications = await _notificationService.GetUserNotificationsAsync(CurrentUserId);
        return Ok(ApiResponse<object>.Ok(notifications));
    }

    /// <summary>Marks a notification as read.</summary>
    [HttpPut("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        try
        {
            await _notificationService.MarkAsReadAsync(id);
            return Ok(ApiResponse<object>.Ok(new { }, "Notification marked as read."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Fail(ex.Message));
        }
    }
}
