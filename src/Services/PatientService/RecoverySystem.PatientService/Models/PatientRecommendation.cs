namespace RecoverySystem.PatientService.Models;

public class PatientRecommendation
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string PatientId { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string Type { get; set; }
    public string Status { get; set; } = "active";
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string CreatedById { get; set; }
    public string CreatedByName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Patient Patient { get; set; }
}