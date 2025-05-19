using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecoverySystem.RecommendationService.DTOs;
using RecoverySystem.RecommendationService.Models;
using RecoverySystem.RecommendationService.Services;
using System.Security.Claims;

namespace RecoverySystem.RecommendationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RecommendationsController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;
        private readonly ILogger<RecommendationsController> _logger;

        public RecommendationsController(
            IRecommendationService recommendationService,
            ILogger<RecommendationsController> logger)
        {
            _recommendationService = recommendationService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetRecommendations([FromQuery] RecommendationStatus? status = null)
        {
            var recommendations = await _recommendationService.GetRecommendationsAsync(status);
            return Ok(recommendations);
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetRecommendationsForPatient(Guid patientId)
        {
            var recommendations = await _recommendationService.GetRecommendationsForPatientAsync(patientId);
            return Ok(recommendations);
        }

        [HttpGet("case/{caseId}")]
        public async Task<IActionResult> GetRecommendationsForCase(Guid caseId)
        {
            var recommendations = await _recommendationService.GetRecommendationsForCaseAsync(caseId);
            return Ok(recommendations);
        }

        [HttpGet("created-by/{createdById}")]
        public async Task<IActionResult> GetRecommendationsCreatedBy(Guid createdById)
        {
            var recommendations = await _recommendationService.GetRecommendationsCreatedByAsync(createdById);
            return Ok(recommendations);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRecommendationById(Guid id)
        {
            var recommendation = await _recommendationService.GetRecommendationByIdAsync(id);
            if (recommendation == null)
            {
                return NotFound(new { message = "Recommendation not found" });
            }

            return Ok(recommendation);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecommendation([FromBody] CreateRecommendationDto createDto)
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

            var recommendation = await _recommendationService.CreateRecommendationAsync(
                createDto,
                Guid.Parse(userId),
                userName);

            return CreatedAtAction(nameof(GetRecommendationById), new { id = recommendation.Id }, recommendation);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRecommendation(Guid id, [FromBody] UpdateRecommendationDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var recommendation = await _recommendationService.UpdateRecommendationAsync(id, updateDto);
            if (recommendation == null)
            {
                return NotFound(new { message = "Recommendation not found" });
            }

            return Ok(recommendation);
        }

        [HttpPatch("{id}/approve")]
        [Authorize(Roles = "admin,doctor")]
        public async Task<IActionResult> ApproveRecommendation(Guid id, [FromBody] ApproveRecommendationDto approveDto)
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

            var recommendation = await _recommendationService.ApproveRecommendationAsync(
                id,
                approveDto,
                Guid.Parse(userId),
                userName);

            if (recommendation == null)
            {
                return NotFound(new { message = "Recommendation not found" });
            }

            return Ok(recommendation);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateRecommendationStatus(Guid id, [FromBody] UpdateStatusDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var recommendation = await _recommendationService.UpdateRecommendationStatusAsync(id, updateDto.Status);
            if (recommendation == null)
            {
                return NotFound(new { message = "Recommendation not found" });
            }

            return Ok(recommendation);
        }
    }

    public class UpdateStatusDto
    {
        public RecommendationStatus Status { get; set; }
    }
}