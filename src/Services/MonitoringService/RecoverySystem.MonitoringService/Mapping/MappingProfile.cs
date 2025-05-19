using AutoMapper;
using RecoverySystem.MonitoringService.DTOs;
using RecoverySystem.MonitoringService.Models;

namespace RecoverySystem.MonitoringService.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // VitalMonitoring mappings
            CreateMap<VitalMonitoring, VitalMonitoringDto>();
            CreateMap<CreateVitalMonitoringDto, VitalMonitoring>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Timestamp, opt => opt.Ignore())
                .ForMember(dest => dest.RecordedById, opt => opt.Ignore())
                .ForMember(dest => dest.RecordedByName, opt => opt.Ignore());

            // Alert mappings
            CreateMap<Alert, AlertDto>();
            CreateMap<CreateAlertDto, Alert>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsResolved, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ResolvedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ResolvedById, opt => opt.Ignore())
                .ForMember(dest => dest.ResolvedByName, opt => opt.Ignore())
                .ForMember(dest => dest.ResolutionNotes, opt => opt.Ignore());

            // SystemHealth mappings
            CreateMap<SystemHealth, SystemHealthDto>();
            CreateMap<CreateSystemHealthDto, SystemHealth>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Timestamp, opt => opt.Ignore());

            // ThresholdConfiguration mappings
            CreateMap<ThresholdConfiguration, ThresholdConfigurationDto>();
            CreateMap<CreateThresholdConfigurationDto, ThresholdConfiguration>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
            CreateMap<UpdateThresholdConfigurationDto, ThresholdConfiguration>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.VitalSign, opt => opt.Ignore())
                .ForMember(dest => dest.IsGlobal, opt => opt.Ignore())
                .ForMember(dest => dest.PatientId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        }
    }
}