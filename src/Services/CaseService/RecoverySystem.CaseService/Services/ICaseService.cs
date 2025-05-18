using RecoverySystem.CaseService.DTOs;
using RecoverySystem.CaseService.Models;

namespace RecoverySystem.CaseService.Services
{
    public interface ICaseService
    {
        Task<IEnumerable<CaseDto>> GetAllCasesAsync();
        Task<CaseDto> GetCaseByIdAsync(Guid id);
        Task<CaseDto> CreateCaseAsync(CreateCaseDto createCaseDto, Guid currentUserId);
        Task<CaseDto> UpdateCaseAsync(Guid id, UpdateCaseDto updateCaseDto);
        Task<CaseDto> UpdateCaseStatusAsync(Guid id, UpdateCaseStatusDto updateStatusDto);
        Task<CaseNoteDto> AddCaseNoteAsync(Guid caseId, CreateCaseNoteDto createNoteDto, Guid currentUserId);
        Task<IEnumerable<CaseNoteDto>> GetCaseNotesAsync(Guid caseId);
        Task<CaseDocumentDto> UploadDocumentAsync(Guid caseId, CaseDocument document, Guid currentUserId);
        Task<IEnumerable<CaseDto>> GetCasesByPatientIdAsync(Guid patientId);
        Task<IEnumerable<CaseDto>> GetCasesByStatusAsync(CaseStatus status);
        Task<IEnumerable<CaseDto>> GetCasesByAssignedToIdAsync(Guid assignedToId);
    }
}