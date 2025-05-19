using System;

namespace RecoverySystem.MonitoringService.DTOs;

public class VitalMonitoringDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public DateTime Timestamp { get; set; }
    public int HeartRate { get; set; }
    public string BloodPressure { get; set; }
    public double Temperature { get; set; }
    public int RespiratoryRate { get; set; }
    public int OxygenSaturation { get; set; }
    public int PainLevel { get; set; }
    public string Source { get; set; }
    public string DeviceId { get; set; }
    public Guid? RecordedById { get; set; }
    public string RecordedByName { get; set; }
}

public class CreateVitalMonitoringDto
{
    public Guid PatientId { get; set; }
    public int HeartRate { get; set; }
    public string BloodPressure { get; set; }
    public double Temperature { get; set; }
    public int RespiratoryRate { get; set; }
    public int OxygenSaturation { get; set; }
    public int PainLevel { get; set; }
    public string Source { get; set; } = "manual";
    public string DeviceId { get; set; }
}