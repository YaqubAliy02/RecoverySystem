using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecoverySystem.NotificationService.DTOs;
using RecoverySystem.NotificationService.Services;
using System.Security.Claims;

namespace RecoverySystem.NotificationService.Controllers;

[ApiController]
[Route("api/notifications/preferences")]
[Authorize]
public class PreferencesController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<PreferencesController> _logger;

    public PreferencesController(
        INotificationService notificationService,
        ILogger<PreferencesController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserPreferences()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User not authenticated" });
        }

        var preferences = await _notificationService.GetUserPreferencesAsync(userId);
        return Ok(preferences);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUserPreferences([FromBody] UpdateNotificationPreferenceDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User not authenticated" });
        }

        var preferences = await _notificationService.UpdateUserPreferencesAsync(userId, updateDto);
        return Ok(preferences);
    }
}