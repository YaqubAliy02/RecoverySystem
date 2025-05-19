using RecoverySystem.RehabilitationService.DTOs;
using RecoverySystem.RehabilitationService.Models;

namespace RecoverySystem.RehabilitationService.Services;

public interface IRehabilitationService
{
    // Rehabilitation Programs
    Task<IEnumerable<RehabilitationProgramDto>> GetRehabilitationProgramsAsync(RehabilitationStatus? status = null);
    Task<IEnumerable<RehabilitationProgramDto>> GetRehabilitationProgramsForPatientAsync(Guid patientId);
    Task<IEnumerable<RehabilitationProgramDto>> GetRehabilitationProgramsForCaseAsync(Guid caseId);
    Task<IEnumerable<RehabilitationProgramDto>> GetRehabilitationProgramsAssignedToAsync(Guid assignedToId);
    Task<RehabilitationProgramDto> GetRehabilitationProgramByIdAsync(Guid id);
    Task<RehabilitationProgramDto> CreateRehabilitationProgramAsync(CreateRehabilitationProgramDto createDto);
    Task<RehabilitationProgramDto> UpdateRehabilitationProgramAsync(Guid id, UpdateRehabilitationProgramDto updateDto);
    Task<RehabilitationProgramDto> UpdateRehabilitationProgramStatusAsync(Guid id, RehabilitationStatus status);

    // Rehabilitation Activities
    Task<IEnumerable<RehabilitationActivityDto>> GetActivitiesForProgramAsync(Guid programId);
    Task<RehabilitationActivityDto> GetActivityByIdAsync(Guid id);
    Task<RehabilitationActivityDto> CreateActivityAsync(CreateRehabilitationActivityDto createDto);
    Task<RehabilitationActivityDto> UpdateActivityAsync(Guid id, UpdateRehabilitationActivityDto updateDto);
    Task<bool> DeleteActivityAsync(Guid id);

    // Rehabilitation Sessions
    Task<IEnumerable<RehabilitationSessionDto>> GetSessionsForProgramAsync(Guid programId);
    Task<RehabilitationSessionDto> GetSessionByIdAsync(Guid id);
    Task<RehabilitationSessionDto> CreateSessionAsync(CreateRehabilitationSessionDto createDto);
    Task<RehabilitationSessionDto> UpdateSessionAsync(Guid id, UpdateRehabilitationSessionDto updateDto, Guid? supervisedById = null, string supervisedByName = null);
    Task<bool> DeleteSessionAsync(Guid id);

    // Progress Reports
    Task<IEnumerable<ProgressReportDto>> GetProgressReportsForProgramAsync(Guid programId);
    Task<IEnumerable<ProgressReportDto>> GetProgressReportsForPatientAsync(Guid patientId);
    Task<ProgressReportDto> GetProgressReportByIdAsync(Guid id);
    Task<ProgressReportDto> CreateProgressReportAsync(CreateProgressReportDto createDto, Guid createdById, string createdByName);
}