using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecoverySystem.IdentityService.Services;

namespace RecoverySystem.IdentityService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly UserProfileService _profileService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        AuthService authService,
        UserProfileService profileService,
        ILogger<UsersController> logger)
    {
        _authService = authService;
        _profileService = profileService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _authService.GetAllUsers();

        var result = users.Select(u => new
        {
            id = u.Id,
            name = u.FullName,
            email = u.Email,
            avatar = u.Avatar,
            role = u.Role,
            createdAt = u.CreatedAt,
            lastLogin = u.LastLogin
        });

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        var user = await _authService.GetUserById(id);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        var profile = await _profileService.GetUserProfileAsync(id);

        return Ok(new
        {
            id = user.Id,
            name = user.FullName,
            email = user.Email,
            avatar = user.Avatar,
            role = user.Role,
            createdAt = user.CreatedAt,
            lastLogin = user.LastLogin,
            profile = profile != null ? new
            {
                id = profile.Id,
                department = profile.Department,
                position = profile.Position,
                contactNumber = profile.ContactNumber,
                address = profile.Address,
                joinDate = profile.JoinDate,
                bio = profile.Bio,
                specializations = profile.Specializations
            } : null
        });
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetUserProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Not authenticated" });
        }

        var profile = await _profileService.GetUserProfileAsync(userId);
        if (profile == null)
        {
            return NotFound(new { message = "Profile not found" });
        }

        return Ok(new
        {
            id = profile.Id,
            userId = profile.UserId,
            department = profile.Department,
            position = profile.Position,
            contactNumber = profile.ContactNumber,
            address = profile.Address,
            joinDate = profile.JoinDate,
            bio = profile.Bio,
            specializations = profile.Specializations
        });
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Not authenticated" });
        }

        var profile = await _profileService.GetUserProfileAsync(userId);
        if (profile == null)
        {
            return NotFound(new { message = "Profile not found" });
        }

        profile.Department = request.Department;
        profile.Position = request.Position;
        profile.ContactNumber = request.ContactNumber;
        profile.Address = request.Address;
        profile.Bio = request.Bio;
        profile.Specializations = request.Specializations;

        var updatedProfile = await _profileService.UpdateUserProfileAsync(profile);

        return Ok(new
        {
            id = updatedProfile.Id,
            userId = updatedProfile.UserId,
            department = updatedProfile.Department,
            position = updatedProfile.Position,
            contactNumber = updatedProfile.ContactNumber,
            address = updatedProfile.Address,
            joinDate = updatedProfile.JoinDate,
            bio = updatedProfile.Bio,
            specializations = updatedProfile.Specializations
        });
    }
}

public class UpdateProfileRequest
{
    public string Department { get; set; }
    public string Position { get; set; }
    public string ContactNumber { get; set; }
    public string Address { get; set; }
    public string Bio { get; set; }
    public List<string> Specializations { get; set; }
}