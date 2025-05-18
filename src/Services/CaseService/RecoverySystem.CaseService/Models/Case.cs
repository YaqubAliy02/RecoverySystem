namespace RecoverySystem.CaseService.Models;

public class Case
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public CaseStatus Status { get; set; }
    public CasePriority Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }

    // Foreign keys
    public Guid PatientId { get; set; }
    public Guid AssignedToId { get; set; }
    public Guid CreatedById { get; set; }

    // Navigation properties
    public virtual ICollection<CaseNote> Notes { get; set; } = new List<CaseNote>();
    public virtual ICollection<CaseDocument> Documents { get; set; } = new List<CaseDocument>();
}