namespace RecoverySystem.PatientService.DTOs;

public class PatientNoteDto
{
    public string Id { get; set; }
    public string PatientId { get; set; }
    public string Content { get; set; }
    public DateTime Date { get; set; }
    public string AuthorId { get; set; }
    public string AuthorName { get; set; }
    public string? AuthorAvatar { get; set; }
    public string? AuthorRole { get; set; }
    public string? Category { get; set; }
}