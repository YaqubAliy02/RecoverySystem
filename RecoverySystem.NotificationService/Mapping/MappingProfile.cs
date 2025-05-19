using AutoMapper;
using RecoverySystem.NotificationService.DTOs;
using RecoverySystem.NotificationService.Models;

namespace RecoverySystem.NotificationService.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Notification mappings
            CreateMap<Notification, NotificationDto>();
            CreateMap<CreateNotificationDto, Notification>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsRead, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ReadAt, opt => opt.Ignore());

            // NotificationPreference mappings
            CreateMap<NotificationPreference, NotificationPreferenceDto>();
            CreateMap<UpdateNotificationPreferenceDto, NotificationPreference>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // NotificationTypePreference mappings
            CreateMap<NotificationTypePreference, NotificationTypePreferenceDto>();
            CreateMap<NotificationTypePreferenceDto, NotificationTypePreference>();
        }
    }
}