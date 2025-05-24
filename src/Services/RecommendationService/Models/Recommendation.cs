namespace RecoverySystem.RecommendationService.Models;

public class Recommendation
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public RecommendationType Type { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; }
    public Guid? CaseId { get; set; }
    public RecommendationPriority Priority { get; set; } = RecommendationPriority.Medium;
    public RecommendationStatus Status { get; set; } = RecommendationStatus.Pending;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Instructions { get; set; }
    public List<string> Tags { get; set; } = new List<string>();
    public Guid CreatedById { get; set; }
    public string CreatedByName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? ApprovedById { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string Notes { get; set; }
}

public enum RecommendationType
{
    Exercise,
    Medication,
    Diet,
    Lifestyle,
    Rehabilitation,
    Therapy,
    Other
}

public enum RecommendationPriority
{
    Low,
    Medium,
    High,
    Critical
}

public enum RecommendationStatus
{
    Pending,
    Approved,
    InProgress,
    Completed,
    Rejected,
    Cancelled
}