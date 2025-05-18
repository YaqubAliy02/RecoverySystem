namespace RecoverySystem.IdentityService.DTOs;

public class RegisterRequestDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string? Role { get; set; }
    public string? Department { get; set; }
    public string? Position { get; set; }
    public string? ContactNumber { get; set; }
    public string? Address { get; set; }
    public string? Bio { get; set; }
    public List<string>? Specializations { get; set; }
}
