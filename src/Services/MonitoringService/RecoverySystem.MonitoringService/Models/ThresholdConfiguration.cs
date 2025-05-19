namespace RecoverySystem.MonitoringService.Models;

public class ThresholdConfiguration
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string VitalSign { get; set; } // HeartRate, BloodPressure, etc.
    public double LowerThreshold { get; set; }
    public double UpperThreshold { get; set; }
    public AlertSeverity Severity { get; set; }
    public bool IsGlobal { get; set; } = true;
    public Guid? PatientId { get; set; } // If not global, applies to specific patient
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}