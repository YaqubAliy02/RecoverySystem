using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecoverySystem.NotificationService.DTOs;
using RecoverySystem.NotificationService.Services;
using System.Security.Claims;

namespace RecoverySystem.NotificationService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(
        INotificationService notificationService,
        ILogger<NotificationsController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserNotifications([FromQuery] bool includeRead = false)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User not authenticated" });
        }

        var notifications = await _notificationService.GetNotificationsForUserAsync(userId, includeRead);
        return Ok(notifications);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetNotificationById(Guid id)
    {
        var notification = await _notificationService.GetNotificationByIdAsync(id);
        if (notification == null)
        {
            return NotFound(new { message = "Notification not found" });
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (notification.TargetUserId != userId && !User.IsInRole("admin"))
        {
            return Forbid();
        }

        return Ok(notification);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationDto createDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var notification = await _notificationService.CreateNotificationAsync(createDto);
        return CreatedAtAction(nameof(GetNotificationById), new { id = notification.Id }, notification);
    }

    [HttpPatch("{id}/read")]
    public async Task<IActionResult> MarkNotificationAsRead(Guid id, [FromBody] MarkNotificationReadDto markReadDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User not authenticated" });
        }

        var notification = await _notificationService.GetNotificationByIdAsync(id);
        if (notification == null)
        {
            return NotFound(new { message = "Notification not found" });
        }

        if (notification.TargetUserId != userId && !User.IsInRole("admin"))
        {
            return Forbid();
        }

        var updatedNotification = await _notificationService.MarkNotificationAsReadAsync(id, markReadDto.IsRead);
        return Ok(updatedNotification);
    }

    [HttpPost("mark-all-read")]
    public async Task<IActionResult> MarkAllNotificationsAsRead()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User not authenticated" });
        }

        var count = await _notificationService.MarkAllNotificationsAsReadAsync(userId);
        return Ok(new { markedAsRead = count });
    }
}