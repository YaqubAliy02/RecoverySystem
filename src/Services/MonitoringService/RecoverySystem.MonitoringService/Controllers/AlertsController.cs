using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecoverySystem.MonitoringService.DTOs;
using RecoverySystem.MonitoringService.Models;
using RecoverySystem.MonitoringService.Services;
using System.Security.Claims;

namespace RecoverySystem.MonitoringService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AlertsController : ControllerBase
{
    private readonly IAlertService _alertService;
    private readonly ILogger<AlertsController> _logger;

    public AlertsController(
        IAlertService alertService,
        ILogger<AlertsController> logger)
    {
        _alertService = alertService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAlerts(
        [FromQuery] bool includeResolved = false,
        [FromQuery] AlertSeverity? severity = null)
    {
        var alerts = await _alertService.GetAlertsAsync(includeResolved);
        return Ok(alerts);
    }

    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetAlertsForPatient(
        Guid patientId,
        [FromQuery] bool includeResolved = false)
    {
        var alerts = await _alertService.GetAlertsForPatientAsync(patientId, includeResolved);
        return Ok(alerts);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAlertById(Guid id)
    {
        var alert = await _alertService.GetAlertByIdAsync(id);
        if (alert == null)
        {
            return NotFound(new { message = "Alert not found" });
        }

        return Ok(alert);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateAlert([FromBody] CreateAlertDto createDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var alert = await _alertService.CreateAlertAsync(createDto);
        return CreatedAtAction(nameof(GetAlertById), new { id = alert.Id }, alert);
    }

    [HttpPatch("{id}/resolve")]
    public async Task<IActionResult> ResolveAlert(Guid id, [FromBody] ResolveAlertDto resolveDto)
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

        var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown User";

        var alert = await _alertService.ResolveAlertAsync(
            id,
            resolveDto,
            Guid.Parse(userId),
            userName);

        if (alert == null)
        {
            return NotFound(new { message = "Alert not found" });
        }

        return Ok(alert);
    }
}