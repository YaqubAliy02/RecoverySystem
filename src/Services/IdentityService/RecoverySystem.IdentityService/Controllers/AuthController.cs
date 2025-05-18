using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RecoverySystem.IdentityService.DTOs;
using RecoverySystem.IdentityService.Models;
using RecoverySystem.IdentityService.Services;

namespace RecoverySystem.IdentityService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly UserProfileService _profileService;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        AuthService authService,
        UserProfileService profileService,
        IMapper mapper,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _profileService = profileService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var (success, token, user) = await _authService.Login(request.Email, request.Password);

        if (!success)
            return Unauthorized(new { message = "Invalid email or password" });

        var userDto = _mapper.Map<UserDto>(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            User = userDto
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        // Map DTO to domain model
        var user = _mapper.Map<ApplicationUser>(request);

        var (success, token, createdUser) = await _authService.Register(
            user.FullName,
            user.Email,
            request.Password,
            request.Role ?? "user");

        if (!success)
            return BadRequest(new { message = "Registration failed" });

        // Create user profile with proper null handling
        var profile = _mapper.Map<UserProfile>(request);
        profile.UserId = createdUser.Id;
        profile.JoinDate = DateTime.UtcNow;

        await _profileService.CreateUserProfileAsync(profile);

        var userDto = _mapper.Map<UserDto>(createdUser);

        return Ok(new AuthResponseDto
        {
            Token = token,
            User = userDto
        });
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Not authenticated" });
        }

        var user = await _authService.GetUserById(userId);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        var profile = await _profileService.GetUserProfileAsync(userId);
        var userDto = _mapper.Map<UserDto>(user);
        var profileDto = profile != null ? _mapper.Map<UserProfileDto>(profile) : null;

        return Ok(new
        {
            user = userDto,
            profile = profileDto
        });
    }
}