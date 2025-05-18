namespace RecoverySystem.PatientService.DTOs;

public class PatientCreateDto
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string? Address { get; set; }
    public string? Status { get; set; }
    public DateTime? LastVisit { get; set; }
    public DateTime? NextAppointment { get; set; }
    public string? Avatar { get; set; }
    public string? MedicalHistory { get; set; }
    public List<string>? Medications { get; set; }
}