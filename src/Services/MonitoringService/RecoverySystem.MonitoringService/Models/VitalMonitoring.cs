using System;

namespace RecoverySystem.MonitoringService.Models;

public class VitalMonitoring
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public int HeartRate { get; set; }
    public string BloodPressure { get; set; }
    public double Temperature { get; set; }
    public int RespiratoryRate { get; set; }
    public int OxygenSaturation { get; set; }
    public int PainLevel { get; set; }
    public string Source { get; set; } = "manual"; // manual, device, etc.
    public string DeviceId { get; set; }
    public Guid? RecordedById { get; set; }
    public string RecordedByName { get; set; }
}