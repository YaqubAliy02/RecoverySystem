namespace RecoverySystem.PatientService.DTOs;

public class PatientNoteCreateDto
{
    public string Content { get; set; }
    public string? Category { get; set; }
    public string? AuthorId { get; set; }
    public string? AuthorName { get; set; }
    public string? AuthorAvatar { get; set; }
    public string? AuthorRole { get; set; }
}
