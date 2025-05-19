namespace RecoverySystem.MonitoringService.Models;

public class Alert
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; }
    public AlertType Type { get; set; }
    public AlertSeverity Severity { get; set; }
    public string Message { get; set; }
    public string Details { get; set; }
    public bool IsResolved { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
    public Guid? ResolvedById { get; set; }
    public string ResolvedByName { get; set; }
    public string ResolutionNotes { get; set; }
}

public enum AlertType
{
    VitalSigns,
    MedicationAdherence,
    AppointmentMissed,
    SystemError,
    Other
}

public enum AlertSeverity
{
    Low,
    Medium,
    High,
    Critical
}