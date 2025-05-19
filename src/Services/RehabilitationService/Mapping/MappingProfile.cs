using AutoMapper;
using RecoverySystem.RehabilitationService.DTOs;
using RecoverySystem.RehabilitationService.Models;

namespace RecoverySystem.RehabilitationService.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // RehabilitationProgram mappings
            CreateMap<RehabilitationProgram, RehabilitationProgramDto>();
            CreateMap<CreateRehabilitationProgramDto, RehabilitationProgram>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => RehabilitationStatus.Planned))
                .ForMember(dest => dest.Activities, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
            CreateMap<UpdateRehabilitationProgramDto, RehabilitationProgram>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PatientId, opt => opt.Ignore())
                .ForMember(dest => dest.PatientName, opt => opt.Ignore())
                .ForMember(dest => dest.CaseId, opt => opt.Ignore())
                .ForMember(dest => dest.Activities, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // RehabilitationActivity mappings
            CreateMap<RehabilitationActivity, RehabilitationActivityDto>();
            CreateMap<CreateRehabilitationActivityDto, RehabilitationActivity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
            CreateMap<UpdateRehabilitationActivityDto, RehabilitationActivity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.RehabilitationProgramId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // RehabilitationSession mappings
            CreateMap<RehabilitationSession, RehabilitationSessionDto>();
            CreateMap<CreateRehabilitationSessionDto, RehabilitationSession>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CompletedDate, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => SessionStatus.Scheduled))
                .ForMember(dest => dest.CompletedActivities, opt => opt.Ignore())
                .ForMember(dest => dest.PainLevel, opt => opt.Ignore())
                .ForMember(dest => dest.FatigueLevel, opt => opt.Ignore())
                .ForMember(dest => dest.SatisfactionLevel, opt => opt.Ignore())
                .ForMember(dest => dest.SupervisedById, opt => opt.Ignore())
                .ForMember(dest => dest.SupervisedByName, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
            CreateMap<UpdateRehabilitationSessionDto, RehabilitationSession>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.RehabilitationProgramId, opt => opt.Ignore())
                .ForMember(dest => dest.SupervisedById, opt => opt.Ignore())
                .ForMember(dest => dest.SupervisedByName, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // ActivityCompletion mappings
            CreateMap<ActivityCompletion, ActivityCompletionDto>();
            CreateMap<ActivityCompletionDto, ActivityCompletion>();

            // ProgressReport mappings
            CreateMap<ProgressReport, ProgressReportDto>();
            CreateMap<CreateProgressReportDto, ProgressReport>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PatientId, opt => opt.Ignore())
                .ForMember(dest => dest.PatientName, opt => opt.Ignore())
                .ForMember(dest => dest.ReportDate, opt => opt.Ignore())
                .ForMember(dest => dest.TotalSessions, opt => opt.Ignore())
                .ForMember(dest => dest.CompletedSessions, opt => opt.Ignore())
                .ForMember(dest => dest.MissedSessions, opt => opt.Ignore())
                .ForMember(dest => dest.ComplianceRate, opt => opt.Ignore())
                .ForMember(dest => dest.ActivityProgress, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByName, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            // ActivityProgress mappings
            CreateMap<ActivityProgress, ActivityProgressDto>();
            CreateMap<ActivityProgressDto, ActivityProgress>();
        }
    }
}