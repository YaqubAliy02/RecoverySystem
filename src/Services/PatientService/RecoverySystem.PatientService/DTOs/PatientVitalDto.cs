namespace RecoverySystem.PatientService.DTOs;

public class PatientVitalDto
{
    public string Id { get; set; }
    public string PatientId { get; set; }
    public DateTime Date { get; set; }
    public int HeartRate { get; set; }
    public string BloodPressure { get; set; }
    public double Temperature { get; set; }
    public int RespiratoryRate { get; set; }
    public int OxygenSaturation { get; set; }
    public int Pain { get; set; }
}
