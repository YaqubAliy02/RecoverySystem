using AutoMapper;
using RecoverySystem.PatientService.DTOs;
using RecoverySystem.PatientService.DTOs.PatientRehabilitations;
using RecoverySystem.PatientService.Models;

namespace RecoverySystem.PatientService.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Patient mappings
        CreateMap<Patient, PatientDto>();
        CreateMap<Patient, PatientDetailDto>()
            .ForMember(dest => dest.Recommendations, opt => opt.Ignore())
            .ForMember(dest => dest.Rehabilitations, opt => opt.Ignore())
            .ForMember(dest => dest.LatestVital, opt => opt.MapFrom(src =>
                src.Vitals.OrderByDescending(v => v.Date).FirstOrDefault()));

        CreateMap<PatientCreateDto, Patient>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Vitals, opt => opt.Ignore())
            .ForMember(dest => dest.Notes, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status ?? "active"))
            .ForMember(dest => dest.Medications, opt => opt.MapFrom(src =>
                src.Medications ?? new List<string>()));

        CreateMap<PatientUpdateDto, Patient>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Vitals, opt => opt.Ignore())
            .ForMember(dest => dest.Notes, opt => opt.Ignore())
            .ForMember(dest => dest.Medications, opt => opt.MapFrom(src =>
                src.Medications ?? new List<string>()));

        // PatientVital mappings
        CreateMap<PatientVital, PatientVitalDto>();
        CreateMap<PatientVitalCreateDto, PatientVital>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PatientId, opt => opt.Ignore())
            .ForMember(dest => dest.Date, opt => opt.Ignore())
            .ForMember(dest => dest.Patient, opt => opt.Ignore());

        // PatientNote mappings
        CreateMap<PatientNote, PatientNoteDto>();
        CreateMap<PatientNoteCreateDto, PatientNote>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PatientId, opt => opt.Ignore())
            .ForMember(dest => dest.Date, opt => opt.Ignore())
            .ForMember(dest => dest.Patient, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category ?? "general"));

        // PatientRecommendation mappings
        CreateMap<PatientRecommendation, PatientRecommendationDto>();
        CreateMap<PatientRecommendationCreateDto, PatientRecommendation>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PatientId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Patient, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status ?? "active"));

        CreateMap<PatientRecommendationUpdateDto, PatientRecommendation>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PatientId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedByName, opt => opt.Ignore())
            .ForMember(dest => dest.Patient, opt => opt.Ignore());

        // PatientRehabilitation mappings
        CreateMap<PatientRehabilitation, PatientRehabilitationDto>();
        CreateMap<PatientRehabilitationCreateDto, PatientRehabilitation>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PatientId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Patient, opt => opt.Ignore())
            .ForMember(dest => dest.Exercises, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status ?? "in-progress"))
            .ForMember(dest => dest.Progress, opt => opt.MapFrom(src => src.Progress ?? 0));

        CreateMap<PatientRehabilitationUpdateDto, PatientRehabilitation>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PatientId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Patient, opt => opt.Ignore())
            .ForMember(dest => dest.Exercises, opt => opt.Ignore());

        // RehabilitationExercise mappings
        CreateMap<RehabilitationExercise, RehabilitationExerciseDto>();
        CreateMap<RehabilitationExerciseCreateDto, RehabilitationExercise>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.RehabilitationId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Rehabilitation, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status ?? "active"));

        CreateMap<RehabilitationExerciseUpdateDto, RehabilitationExercise>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.RehabilitationId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Rehabilitation, opt => opt.Ignore());
    }
}