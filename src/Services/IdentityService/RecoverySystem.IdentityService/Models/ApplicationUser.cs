// Models/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;

namespace RecoverySystem.IdentityService.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; }
    public string? Avatar { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLogin { get; set; }
}