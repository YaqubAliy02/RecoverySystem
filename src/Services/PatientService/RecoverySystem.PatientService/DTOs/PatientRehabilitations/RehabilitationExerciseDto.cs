namespace RecoverySystem.PatientService.DTOs.PatientRehabilitations;

public class RehabilitationExerciseDto
{
    public string Id { get; set; }
    public string RehabilitationId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public int DurationMinutes { get; set; }
    public int FrequencyPerWeek { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}