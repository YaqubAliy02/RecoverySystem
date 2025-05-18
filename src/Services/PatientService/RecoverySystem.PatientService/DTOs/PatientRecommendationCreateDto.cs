namespace RecoverySystem.PatientService.DTOs;

public class PatientRecommendationCreateDto
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public string Type { get; set; }
    public string? Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? CreatedById { get; set; }
    public string? CreatedByName { get; set; }
}
