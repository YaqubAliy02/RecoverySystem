using AutoMapper;
using RecoverySystem.CaseService.DTOs;
using RecoverySystem.CaseService.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RecoverySystem.CaseService.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Case mappings
            CreateMap<Case, CaseDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()))
                .ForMember(dest => dest.PatientName, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedToName, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByName, opt => opt.Ignore());

            CreateMap<CreateCaseDto, Case>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => CaseStatus.Open))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => System.DateTime.UtcNow))
                .ForMember(dest => dest.Notes, opt => opt.Ignore())
                .ForMember(dest => dest.Documents, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ClosedAt, opt => opt.Ignore());

            CreateMap<UpdateCaseDto, Case>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.PatientId, opt => opt.Ignore())
                .ForMember(dest => dest.Notes, opt => opt.Ignore())
                .ForMember(dest => dest.Documents, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => System.DateTime.UtcNow))
                .ForMember(dest => dest.ClosedAt, opt => opt.Ignore());

            // CaseNote mappings
            CreateMap<CaseNote, CaseNoteDto>()
                .ForMember(dest => dest.CreatedByName, opt => opt.Ignore());

            CreateMap<CreateCaseNoteDto, CaseNote>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => System.DateTime.UtcNow))
                .ForMember(dest => dest.CaseId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.Case, opt => opt.Ignore());

            // CaseDocument mappings
            CreateMap<CaseDocument, CaseDocumentDto>()
                .ForMember(dest => dest.UploadedByName, opt => opt.Ignore());
        }
    }
}