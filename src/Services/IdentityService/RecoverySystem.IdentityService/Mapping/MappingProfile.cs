using AutoMapper;
using RecoverySystem.IdentityService.DTOs;
using RecoverySystem.IdentityService.Models;

namespace RecoverySystem.IdentityService.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<ApplicationUser, UserDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName));

        // UserProfile mappings
        CreateMap<UserProfile, UserProfileDto>();

        // Registration mapping
        CreateMap<RegisterRequestDto, ApplicationUser>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

        CreateMap<RegisterRequestDto, UserProfile>()
            .ForMember(dest => dest.Specializations, opt => opt.MapFrom(src =>
                src.Specializations != null ? src.Specializations : new List<string>()));
    }
}