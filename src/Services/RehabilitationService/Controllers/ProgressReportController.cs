using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecoverySystem.RehabilitationService.DTOs;
using RecoverySystem.RehabilitationService.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RecoverySystem.RehabilitationService.Controllers
{
    [ApiController]
    [Route("api/rehabilitation/progress-reports")]
    [Authorize]
    public class ProgressReportsController : ControllerBase
    {
        private readonly IRehabilitationService _rehabilitationService;
        private readonly ILogger<ProgressReportsController> _logger;

        public ProgressReportsController(
            IRehabilitationService rehabilitationService,
            ILogger<ProgressReportsController> logger)
        {
            _rehabilitationService = rehabilitationService;
            _logger = logger;
        }

        [HttpGet("program/{programId}")]
        public async Task<IActionResult> GetProgressReportsForProgram(Guid programId)
        {
            var reports = await _rehabilitationService.GetProgressReportsForProgramAsync(programId);
            return Ok(reports);
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetProgressReportsForPatient(Guid patientId)
        {
            var reports = await _rehabilitationService.GetProgressReportsForPatientAsync(patientId);
            return Ok(reports);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProgressReportById(Guid id)
        {
            var report = await _rehabilitationService.GetProgressReportByIdAsync(id);
            if (report == null)
            {
                return NotFound(new { message = "Progress report not found" });
            }

            return Ok(report);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProgressReport([FromBody] CreateProgressReportDto createDto)
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

            var report = await _rehabilitationService.CreateProgressReportAsync(
                createDto,
                Guid.Parse(userId),
                userName);

            if (report == null)
            {
                return BadRequest(new { message = "Rehabilitation program not found" });
            }

            return CreatedAtAction(nameof(GetProgressReportById), new { id = report.Id }, report);
        }
    }
}