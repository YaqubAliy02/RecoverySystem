using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RecoverySystem.IdentityService.Models;

namespace RecoverySystem.IdentityService.Services;

public class AuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;
    private readonly EventPublisher _eventPublisher;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        ILogger<AuthService> logger,
        EventPublisher eventPublisher)
    {
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
        _eventPublisher = eventPublisher;
    }

    public async Task<(bool Success, string Token, ApplicationUser User)> Login(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            _logger.LogWarning("Login attempt failed: User not found for email {Email}", email);
            return (false, string.Empty, null);
        }

        var result = await _userManager.CheckPasswordAsync(user, password);
        if (!result)
        {
            _logger.LogWarning("Login attempt failed: Invalid password for user {Email}", email);
            return (false, string.Empty, null);
        }

        user.LastLogin = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        var token = GenerateJwtToken(user);
        _logger.LogInformation("User {Email} logged in successfully", email);

        // Publish login event
        await _eventPublisher.PublishUserLoggedInEventAsync(user);

        return (true, token, user);
    }

    public async Task<(bool Success, string Token, ApplicationUser User)> Register(string fullName, string email, string password, string role = "user")
    {
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            _logger.LogWarning("Registration failed: Email {Email} already exists", email);
            return (false, string.Empty, null);
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            Role = role,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogWarning("Registration failed: {Errors}", errors);
            return (false, string.Empty, null);
        }

        var token = GenerateJwtToken(user);
        _logger.LogInformation("User {Email} registered successfully", email);
        await _eventPublisher.PublishUserCreatedEventAsync(user);
        return (true, token, user);
    }

    public async Task<ApplicationUser> GetUserById(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    public async Task<ApplicationUser> GetUserByEmail(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<List<ApplicationUser>> GetAllUsers()
    {
        return await _userManager.Users.ToListAsync();
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("name", user.FullName),
            new Claim("role", user.Role ?? "user")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["Jwt:ExpireDays"]));

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}