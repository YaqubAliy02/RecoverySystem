namespace RecoverySystem.PatientService.DTOs.PatientRehabilitations;

public class PatientRehabilitationDto
{
    public string Id { get; set; }
    public string PatientId { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; }
    public int Progress { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? AssignedToId { get; set; }
    public string? AssignedToName { get; set; }
    public List<RehabilitationExerciseDto> Exercises { get; set; } = new List<RehabilitationExerciseDto>();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

