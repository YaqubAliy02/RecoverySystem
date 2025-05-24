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
    [Route("api/recommendations/templates")]
    [Authorize]
    public class TemplatesController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;
        private readonly ILogger<TemplatesController> _logger;

        public TemplatesController(
            IRecommendationService recommendationService,
            ILogger<TemplatesController> logger)
        {
            _recommendationService = recommendationService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetTemplates([FromQuery] bool includeInactive = false)
        {
            var templates = await _recommendationService.GetTemplatesAsync(includeInactive);
            return Ok(templates);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTemplateById(Guid id)
        {
            var template = await _recommendationService.GetTemplateByIdAsync(id);
            if (template == null)
            {
                return NotFound(new { message = "Template not found" });
            }

            return Ok(template);
        }

        [HttpPost]
        [Authorize(Roles = "admin,doctor")]
        public async Task<IActionResult> CreateTemplate([FromBody] CreateRecommendationTemplateDto createDto)
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

            var template = await _recommendationService.CreateTemplateAsync(
                createDto,
                Guid.Parse(userId),
                userName);

            return CreatedAtAction(nameof(GetTemplateById), new { id = template.Id }, template);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin,doctor")]
        public async Task<IActionResult> UpdateTemplate(Guid id, [FromBody] UpdateRecommendationTemplateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var template = await _recommendationService.UpdateTemplateAsync(id, updateDto);
            if (template == null)
            {
                return NotFound(new { message = "Template not found" });
            }

            return Ok(template);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteTemplate(Guid id)
        {
            var result = await _recommendationService.DeleteTemplateAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Template not found" });
            }

            return NoContent();
        }
    }
}