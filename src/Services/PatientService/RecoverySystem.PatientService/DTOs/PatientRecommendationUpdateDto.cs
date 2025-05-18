namespace RecoverySystem.PatientService.DTOs;

public class PatientRecommendationUpdateDto
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public string Type { get; set; }
    public string Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
