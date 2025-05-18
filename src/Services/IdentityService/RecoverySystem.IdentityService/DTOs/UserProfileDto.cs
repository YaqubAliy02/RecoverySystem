public class UserProfileDto
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string? Department { get; set; }
    public string? Position { get; set; }
    public string? ContactNumber { get; set; }
    public string? Address { get; set; }
    public DateTime JoinDate { get; set; }
    public string? Bio { get; set; }
    public List<string> Specializations { get; set; } = new List<string>();
}