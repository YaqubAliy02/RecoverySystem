namespace RecoverySystem.IdentityService.DTOs;

public class UserDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string? Avatar { get; set; }
    public string? Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
}
