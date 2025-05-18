namespace RecoverySystem.PatientService.DTOs.PatientRehabilitations;

public class RehabilitationExerciseUpdateDto
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public int DurationMinutes { get; set; }
    public int FrequencyPerWeek { get; set; }
    public string Status { get; set; }
}
