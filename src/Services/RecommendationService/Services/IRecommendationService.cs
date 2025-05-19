using RecoverySystem.RecommendationService.DTOs;
using RecoverySystem.RecommendationService.Models;

namespace RecoverySystem.RecommendationService.Services;

public interface IRecommendationService
{
    // Recommendations
    Task<IEnumerable<RecommendationDto>> GetRecommendationsAsync(RecommendationStatus? status = null);
    Task<IEnumerable<RecommendationDto>> GetRecommendationsForPatientAsync(Guid patientId);
    Task<IEnumerable<RecommendationDto>> GetRecommendationsForCaseAsync(Guid caseId);
    Task<IEnumerable<RecommendationDto>> GetRecommendationsCreatedByAsync(Guid createdById);
    Task<RecommendationDto> GetRecommendationByIdAsync(Guid id);
    Task<RecommendationDto> CreateRecommendationAsync(CreateRecommendationDto createDto, Guid currentUserId, string currentUserName);
    Task<RecommendationDto> UpdateRecommendationAsync(Guid id, UpdateRecommendationDto updateDto);
    Task<RecommendationDto> ApproveRecommendationAsync(Guid id, ApproveRecommendationDto approveDto, Guid currentUserId, string currentUserName);
    Task<RecommendationDto> UpdateRecommendationStatusAsync(Guid id, RecommendationStatus status);

    // Recommendation Feedback
    Task<IEnumerable<RecommendationFeedbackDto>> GetFeedbackForRecommendationAsync(Guid recommendationId);
    Task<RecommendationFeedbackDto> GetFeedbackByIdAsync(Guid id);
    Task<RecommendationFeedbackDto> CreateFeedbackAsync(CreateRecommendationFeedbackDto createDto, Guid currentUserId, string currentUserName);

    // Recommendation Templates
    Task<IEnumerable<RecommendationTemplateDto>> GetTemplatesAsync(bool includeInactive = false);
    Task<RecommendationTemplateDto> GetTemplateByIdAsync(Guid id);
    Task<RecommendationTemplateDto> CreateTemplateAsync(CreateRecommendationTemplateDto createDto, Guid currentUserId, string currentUserName);
    Task<RecommendationTemplateDto> UpdateTemplateAsync(Guid id, UpdateRecommendationTemplateDto updateDto);
    Task<bool> DeleteTemplateAsync(Guid id);
}