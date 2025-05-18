namespace RecoverySystem.PatientService.DTOs;

public class PatientRecommendationDto
{
    public string Id { get; set; }
    public string PatientId { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string Type { get; set; }
    public string Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string CreatedById { get; set; }
    public string CreatedByName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
