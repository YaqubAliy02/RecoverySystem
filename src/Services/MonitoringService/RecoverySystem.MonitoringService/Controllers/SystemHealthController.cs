using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecoverySystem.MonitoringService.DTOs;
using RecoverySystem.MonitoringService.Services;

namespace RecoverySystem.MonitoringService.Controllers
{
    [ApiController]
    [Route("api/system-health")]
    [Authorize]
    public class SystemHealthController : ControllerBase
    {
        private readonly IMonitoringService _monitoringService;
        private readonly ILogger<SystemHealthController> _logger;

        public SystemHealthController(
            IMonitoringService monitoringService,
            ILogger<SystemHealthController> logger)
        {
            _monitoringService = monitoringService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetSystemHealths()
        {
            var systemHealths = await _monitoringService.GetSystemHealthsAsync();
            return Ok(systemHealths);
        }

        [HttpGet("{serviceName}")]
        public async Task<IActionResult> GetSystemHealthByServiceName(string serviceName)
        {
            var systemHealth = await _monitoringService.GetSystemHealthByServiceNameAsync(serviceName);
            if (systemHealth == null)
            {
                return NotFound(new { message = "System health for service not found" });
            }

            return Ok(systemHealth);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> RecordSystemHealth([FromBody] CreateSystemHealthDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var systemHealth = await _monitoringService.RecordSystemHealthAsync(createDto);
            return CreatedAtAction(nameof(GetSystemHealthByServiceName), new { serviceName = systemHealth.ServiceName }, systemHealth);
        }
    }
}