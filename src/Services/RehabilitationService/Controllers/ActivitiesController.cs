using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecoverySystem.RehabilitationService.DTOs;
using RecoverySystem.RehabilitationService.Services;
using System;
using System.Threading.Tasks;

namespace RecoverySystem.RehabilitationService.Controllers
{
    [ApiController]
    [Route("api/rehabilitation/activities")]
    [Authorize]
    public class ActivitiesController : ControllerBase
    {
        private readonly IRehabilitationService _rehabilitationService;
        private readonly ILogger<ActivitiesController> _logger;

        public ActivitiesController(
            IRehabilitationService rehabilitationService,
            ILogger<ActivitiesController> logger)
        {
            _rehabilitationService = rehabilitationService;
            _logger = logger;
        }

        [HttpGet("program/{programId}")]
        public async Task<IActionResult> GetActivitiesForProgram(Guid programId)
        {
            var activities = await _rehabilitationService.GetActivitiesForProgramAsync(programId);
            return Ok(activities);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetActivityById(Guid id)
        {
            var activity = await _rehabilitationService.GetActivityByIdAsync(id);
            if (activity == null)
            {
                return NotFound(new { message = "Rehabilitation activity not found" });
            }

            return Ok(activity);
        }

        [HttpPost]
        public async Task<IActionResult> CreateActivity([FromBody] CreateRehabilitationActivityDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var activity = await _rehabilitationService.CreateActivityAsync(createDto);
            if (activity == null)
            {
                return BadRequest(new { message = "Rehabilitation program not found" });
            }

            return CreatedAtAction(nameof(GetActivityById), new { id = activity.Id }, activity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateActivity(Guid id, [FromBody] UpdateRehabilitationActivityDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var activity = await _rehabilitationService.UpdateActivityAsync(id, updateDto);
            if (activity == null)
            {
                return NotFound(new { message = "Rehabilitation activity not found" });
            }

            return Ok(activity);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity(Guid id)
        {
            var result = await _rehabilitationService.DeleteActivityAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Rehabilitation activity not found" });
            }

            return NoContent();
        }
    }
}