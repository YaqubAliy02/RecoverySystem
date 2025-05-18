
Snamespace RecoverySystem.CaseService.Models;

public class CaseNote
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string CaseId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string AuthorId { get; set; }
    public string AuthorName { get; set; }
    public string AuthorAvatar { get; set; }
    public string AuthorRole { get; set; }
    public Case Case { get; set; }
}