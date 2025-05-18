namespace RecoverySystem.PatientService.DTOs.PatientRehabilitations;

public class PatientRehabilitationCreateDto
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public int? Progress { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? AssignedToId { get; set; }
    public string? AssignedToName { get; set; }
    public List<RehabilitationExerciseCreateDto>? Exercises { get; set; }
}

