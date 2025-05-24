using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecoverySystem.RehabilitationService.DTOs;
using RecoverySystem.RehabilitationService.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RecoverySystem.RehabilitationService.Controllers
{
    [ApiController]
    [Route("api/rehabilitation/sessions")]
    [Authorize]
    public class SessionsController : ControllerBase
    {
        private readonly IRehabilitationService _rehabilitationService;
        private readonly ILogger<SessionsController> _logger;

        public SessionsController(
            IRehabilitationService rehabilitationService,
            ILogger<SessionsController> logger)
        {
            _rehabilitationService = rehabilitationService;
            _logger = logger;
        }

        [HttpGet("program/{programId}")]
        public async Task<IActionResult> GetSessionsForProgram(Guid programId)
        {
            var sessions = await _rehabilitationService.GetSessionsForProgramAsync(programId);
            return Ok(sessions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSessionById(Guid id)
        {
            var session = await _rehabilitationService.GetSessionByIdAsync(id);
            if (session == null)
            {
                return NotFound(new { message = "Rehabilitation session not found" });
            }

            return Ok(session);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSession([FromBody] CreateRehabilitationSessionDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var session = await _rehabilitationService.CreateSessionAsync(createDto);
            if (session == null)
            {
                return BadRequest(new { message = "Rehabilitation program not found" });
            }

            return CreatedAtAction(nameof(GetSessionById), new { id = session.Id }, session);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSession(Guid id, [FromBody] UpdateRehabilitationSessionDto updateDto)
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

            var session = await _rehabilitationService.UpdateSessionAsync(
                id,
                updateDto,
                Guid.Parse(userId),
                userName);

            if (session == null)
            {
                return NotFound(new { message = "Rehabilitation session not found" });
            }

            return Ok(session);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSession(Guid id)
        {
            var result = await _rehabilitationService.DeleteSessionAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Rehabilitation session not found" });
            }

            return NoContent();
        }
    }
}