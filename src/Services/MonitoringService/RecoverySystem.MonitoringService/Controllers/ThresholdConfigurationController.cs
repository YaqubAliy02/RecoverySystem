using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecoverySystem.MonitoringService.DTOs;
using RecoverySystem.MonitoringService.Services;
using System;
using System.Threading.Tasks;

namespace RecoverySystem.MonitoringService.Controllers
{
    [ApiController]
    [Route("api/threshold-configurations")]
    [Authorize]
    public class ThresholdConfigurationsController : ControllerBase
    {
        private readonly IMonitoringService _monitoringService;
        private readonly ILogger<ThresholdConfigurationsController> _logger;

        public ThresholdConfigurationsController(
            IMonitoringService monitoringService,
            ILogger<ThresholdConfigurationsController> logger)
        {
            _monitoringService = monitoringService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetThresholdConfigurations([FromQuery] bool includeInactive = false)
        {
            var thresholds = await _monitoringService.GetThresholdConfigurationsAsync(includeInactive);
            return Ok(thresholds);
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetThresholdConfigurationsForPatient(
            Guid patientId,
            [FromQuery] bool includeInactive = false)
        {
            var thresholds = await _monitoringService.GetThresholdConfigurationsForPatientAsync(patientId, includeInactive);
            return Ok(thresholds);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetThresholdConfigurationById(Guid id)
        {
            var threshold = await _monitoringService.GetThresholdConfigurationByIdAsync(id);
            if (threshold == null)
            {
                return NotFound(new { message = "Threshold configuration not found" });
            }

            return Ok(threshold);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateThresholdConfiguration([FromBody] CreateThresholdConfigurationDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var threshold = await _monitoringService.CreateThresholdConfigurationAsync(createDto);
            return CreatedAtAction(nameof(GetThresholdConfigurationById), new { id = threshold.Id }, threshold);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateThresholdConfiguration(Guid id, [FromBody] UpdateThresholdConfigurationDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var threshold = await _monitoringService.UpdateThresholdConfigurationAsync(id, updateDto);
            if (threshold == null)
            {
                return NotFound(new { message = "Threshold configuration not found" });
            }

            return Ok(threshold);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteThresholdConfiguration(Guid id)
        {
            var result = await _monitoringService.DeleteThresholdConfigurationAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Threshold configuration not found" });
            }

            return NoContent();
        }
    }
}