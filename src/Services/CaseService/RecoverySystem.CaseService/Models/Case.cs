// Models/Case.cs
namespace RecoverySystem.CaseService.Models;

public class Case
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string CaseNumber { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string PatientId { get; set; }
    public string PatientName { get; set; }
    public string Status { get; set; } = "open";
    public string Priority { get; set; } = "medium";
    public string AssignedToId { get; set; }
    public string AssignedToName { get; set; }
    public string AssignedToAvatar { get; set; }
    public string Category { get; set; }
    public string Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ClosedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public List<CaseNote> Notes { get; set; } = new List<CaseNote>();
    public List<CaseDocument> Documents { get; set; } = new List<CaseDocument>();
    public List<CaseTimelineEvent> TimelineEvents { get; set; } = new List<CaseTimelineEvent>();
}