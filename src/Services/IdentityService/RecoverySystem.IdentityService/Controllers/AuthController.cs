// Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using RecoverySystem.IdentityService.Services;

namespace RecoverySystem.IdentityService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var (success, token, user) = await _authService.Login(request.Email, request.Password);

        if (!success)
            return Unauthorized(new { message = "Invalid email or password" });

        return Ok(new
        {
            token,
            user = new
            {
                id = user.Id,
                name = user.FullName,
                email = user.Email,
                avatar = user.Avatar
            }
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var (success, token, user) = await _authService.Register(request.Name, request.Email, request.Password);

        if (!success)
            return BadRequest(new { message = "Registration failed" });

        return Ok(new
        {
            token,
            user = new
            {
                id = user.Id,
                name = user.FullName,
                email = user.Email,
                avatar = user.Avatar
            }
        });
    }
}

public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class RegisterRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}