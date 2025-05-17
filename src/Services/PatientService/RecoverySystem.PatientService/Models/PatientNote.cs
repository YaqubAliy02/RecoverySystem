using RecoverySystem.PatientService.Models;

public class PatientNote
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string PatientId { get; set; }
    public string Content { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string AuthorId { get; set; }
    public string AuthorName { get; set; }
    public string AuthorAvatar { get; set; }
    public string AuthorRole { get; set; }
    public string Category { get; set; }
    public Patient Patient { get; set; }
}