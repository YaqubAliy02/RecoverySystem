using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecoverySystem.MonitoringService.DTOs;
using RecoverySystem.MonitoringService.Services;
using System.Security.Claims;

namespace RecoverySystem.MonitoringService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VitalMonitoringsController : ControllerBase
{
    private readonly IMonitoringService _monitoringService;
    private readonly ILogger<VitalMonitoringsController> _logger;

    public VitalMonitoringsController(
        IMonitoringService monitoringService,
        ILogger<VitalMonitoringsController> logger)
    {
        _monitoringService = monitoringService;
        _logger = logger;
    }

    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetVitalMonitoringsForPatient(
        Guid patientId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var vitalMonitorings = await _monitoringService.GetVitalMonitoringsForPatientAsync(patientId, startDate, endDate);
        return Ok(vitalMonitorings);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetVitalMonitoringById(Guid id)
    {
        var vitalMonitoring = await _monitoringService.GetVitalMonitoringByIdAsync(id);
        if (vitalMonitoring == null)
        {
            return NotFound(new { message = "Vital monitoring not found" });
        }

        return Ok(vitalMonitoring);
    }

    [HttpPost]
    public async Task<IActionResult> RecordVitalMonitoring([FromBody] CreateVitalMonitoringDto createDto)
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

        var vitalMonitoring = await _monitoringService.RecordVitalMonitoringAsync(
            createDto,
            Guid.Parse(userId),
            userName);

        return CreatedAtAction(nameof(GetVitalMonitoringById), new { id = vitalMonitoring.Id }, vitalMonitoring);
    }

    [HttpGet("latest")]
    public async Task<IActionResult> GetLatestVitalMonitorings([FromQuery] int count = 10)
    {
        var vitalMonitorings = await _monitoringService.GetLatestVitalMonitoringsAsync(count);
        return Ok(vitalMonitorings);
    }
}