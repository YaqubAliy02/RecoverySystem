namespace RecoverySystem.RehabilitationService.Models;

public class RehabilitationActivity
{
    public Guid Id { get; set; }
    public Guid RehabilitationProgramId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public ActivityType Type { get; set; }
    public string Instructions { get; set; }
    public int DurationMinutes { get; set; }
    public int Frequency { get; set; } // Times per week
    public int Sets { get; set; }
    public int Repetitions { get; set; }
    public string VideoUrl { get; set; }
    public string ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public enum ActivityType
{
    Exercise,
    Stretching,
    Therapy,
    Medication,
    Rest,
    Other
}