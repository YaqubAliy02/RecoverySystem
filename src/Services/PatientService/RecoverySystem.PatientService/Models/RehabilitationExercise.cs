namespace RecoverySystem.PatientService.Models;

public class RehabilitationExercise
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string RehabilitationId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public int DurationMinutes { get; set; }
    public int FrequencyPerWeek { get; set; }
    public string Status { get; set; } = "active";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public PatientRehabilitation Rehabilitation { get; set; }
}