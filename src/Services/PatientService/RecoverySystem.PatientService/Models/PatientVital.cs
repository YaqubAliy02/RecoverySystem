namespace RecoverySystem.PatientService.Models;

public class PatientVital
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string PatientId { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public int HeartRate { get; set; }
    public string BloodPressure { get; set; }
    public double Temperature { get; set; }
    public int RespiratoryRate { get; set; }
    public int OxygenSaturation { get; set; }
    public int Pain { get; set; }
    public Patient Patient { get; set; }
}