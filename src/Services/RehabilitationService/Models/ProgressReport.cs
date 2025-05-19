namespace RecoverySystem.RehabilitationService.Models;

public class ProgressReport
{
    public Guid Id { get; set; }
    public Guid RehabilitationProgramId { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; }
    public DateTime ReportDate { get; set; } = DateTime.UtcNow;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalSessions { get; set; }
    public int CompletedSessions { get; set; }
    public int MissedSessions { get; set; }
    public double ComplianceRate { get; set; } // Percentage
    public List<ActivityProgress> ActivityProgress { get; set; } = new List<ActivityProgress>();
    public string Assessment { get; set; }
    public string Recommendations { get; set; }
    public Guid CreatedById { get; set; }
    public string CreatedByName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class ActivityProgress
{
    public Guid ActivityId { get; set; }
    public string ActivityTitle { get; set; }
    public int TotalCompletions { get; set; }
    public int ExpectedCompletions { get; set; }
    public double ComplianceRate { get; set; } // Percentage
    public string Notes { get; set; }
}