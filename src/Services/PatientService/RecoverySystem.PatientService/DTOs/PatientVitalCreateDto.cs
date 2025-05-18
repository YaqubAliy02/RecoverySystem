namespace RecoverySystem.PatientService.DTOs;

public class PatientVitalCreateDto
{
    public int HeartRate { get; set; }
    public string BloodPressure { get; set; }
    public double Temperature { get; set; }
    public int RespiratoryRate { get; set; }
    public int OxygenSaturation { get; set; }
    public int Pain { get; set; }
}