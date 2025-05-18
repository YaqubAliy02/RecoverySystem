namespace RecoverySystem.PatientService.Models;

public class Patient
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string? Address { get; set; }
    public string Status { get; set; } = "active";
    public DateTime? LastVisit { get; set; }
    public DateTime? NextAppointment { get; set; }
    public string? Avatar { get; set; }
    public string? MedicalHistory { get; set; }
    public List<string> Medications { get; set; } = new List<string>();
    public List<PatientVital> Vitals { get; set; } = new List<PatientVital>();
    public List<PatientNote> Notes { get; set; } = new List<PatientNote>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}