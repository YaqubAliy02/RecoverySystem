namespace RecoverySystem.PatientService.Models;

public class PatientRehabilitation
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string PatientId { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = "in-progress";
    public int Progress { get; set; } = 0;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? AssignedToId { get; set; }
    public string? AssignedToName { get; set; }
    public List<RehabilitationExercise> Exercises { get; set; } = new List<RehabilitationExercise>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Patient Patient { get; set; }
}