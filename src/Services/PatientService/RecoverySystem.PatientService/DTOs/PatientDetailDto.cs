using RecoverySystem.PatientService.DTOs.PatientRehabilitations;

namespace RecoverySystem.PatientService.DTOs;

public class PatientDetailDto : PatientDto
{
    public List<PatientVitalDto> Vitals { get; set; } = new List<PatientVitalDto>();
    public List<PatientNoteDto> Notes { get; set; } = new List<PatientNoteDto>();
    public List<PatientRecommendationDto> Recommendations { get; set; } = new List<PatientRecommendationDto>();
    public List<PatientRehabilitationDto> Rehabilitations { get; set; } = new List<PatientRehabilitationDto>();
}
