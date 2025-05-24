using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecoverySystem.RecommendationService.DTOs;
using RecoverySystem.RecommendationService.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RecoverySystem.RecommendationService.Controllers
{
    [ApiController]
    [Route("api/recommendations/feedback")]
    [Authorize]
    public class FeedbackController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;
        private readonly ILogger<FeedbackController> _logger;

        public FeedbackController(
            IRecommendationService recommendationService,
            ILogger<FeedbackController> logger)
        {
            _recommendationService = recommendationService;
            _logger = logger;
        }

        [HttpGet("recommendation/{recommendationId}")]
        public async Task<IActionResult> GetFeedbackForRecommendation(Guid recommendationId)
        {
            var feedback = await _recommendationService.GetFeedbackForRecommendationAsync(recommendationId);
            return Ok(feedback);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFeedbackById(Guid id)
        {
            var feedback = await _recommendationService.GetFeedbackByIdAsync(id);
            if (feedback == null)
            {
                return NotFound(new { message = "Feedback not found" });
            }

            return Ok(feedback);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFeedback([FromBody] CreateRecommendationFeedbackDto createDto)
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

            var userName = User.FindFirst(JwtRegisteredClaimNames.Name)?.Value ?? "Unknown User";

            var feedback = await _recommendationService.CreateFeedbackAsync(
                createDto,
                Guid.Parse(userId),
                userName);

            if (feedback == null)
            {
                return BadRequest(new { message = "Recommendation not found" });
            }

            return CreatedAtAction(nameof(GetFeedbackById), new { id = feedback.Id }, feedback);
        }
    }
}