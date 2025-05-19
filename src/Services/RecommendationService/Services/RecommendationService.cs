using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecoverySystem.RecommendationService.Data;
using RecoverySystem.RecommendationService.DTOs;
using RecoverySystem.RecommendationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecoverySystem.RecommendationService.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly RecommendationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<RecommendationService> _logger;
        private readonly EventPublisher _eventPublisher;

        public RecommendationService(
            RecommendationDbContext dbContext,
            IMapper mapper,
            ILogger<RecommendationService> logger,
            EventPublisher eventPublisher)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        #region Recommendations

        public async Task<IEnumerable<RecommendationDto>> GetRecommendationsAsync(RecommendationStatus? status = null)
        {
            var query = _dbContext.Recommendations.AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(r => r.Status == status.Value);
            }

            var recommendations = await query
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<RecommendationDto>>(recommendations);
        }

        public async Task<IEnumerable<RecommendationDto>> GetRecommendationsForPatientAsync(Guid patientId)
        {
            var recommendations = await _dbContext.Recommendations
                .Where(r => r.PatientId == patientId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<RecommendationDto>>(recommendations);
        }

        public async Task<IEnumerable<RecommendationDto>> GetRecommendationsForCaseAsync(Guid caseId)
        {
            var recommendations = await _dbContext.Recommendations
                .Where(r => r.CaseId == caseId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<RecommendationDto>>(recommendations);
        }

        public async Task<IEnumerable<RecommendationDto>> GetRecommendationsCreatedByAsync(Guid createdById)
        {
            var recommendations = await _dbContext.Recommendations
                .Where(r => r.CreatedById == createdById)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<RecommendationDto>>(recommendations);
        }

        public async Task<RecommendationDto> GetRecommendationByIdAsync(Guid id)
        {
            var recommendation = await _dbContext.Recommendations.FindAsync(id);
            if (recommendation == null)
            {
                _logger.LogWarning("Recommendation with ID {Id} not found", id);
                return null;
            }

            return _mapper.Map<RecommendationDto>(recommendation);
        }

        public async Task<RecommendationDto> CreateRecommendationAsync(CreateRecommendationDto createDto, Guid currentUserId, string currentUserName)
        {
            var recommendation = _mapper.Map<Recommendation>(createDto);
            recommendation.Id = Guid.NewGuid();
            recommendation.Status = RecommendationStatus.Pending;
            recommendation.CreatedById = currentUserId;
            recommendation.CreatedByName = currentUserName;
            recommendation.CreatedAt = DateTime.UtcNow;

            await _dbContext.Recommendations.AddAsync(recommendation);
            await _dbContext.SaveChangesAsync();

            // Publish event
            try
            {
                await _eventPublisher.PublishRecommendationCreatedEventAsync(recommendation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish RecommendationCreatedEvent for recommendation {Id}", recommendation.Id);
            }

            return _mapper.Map<RecommendationDto>(recommendation);
        }

        public async Task<RecommendationDto> UpdateRecommendationAsync(Guid id, UpdateRecommendationDto updateDto)
        {
            var recommendation = await _dbContext.Recommendations.FindAsync(id);
            if (recommendation == null)
            {
                _logger.LogWarning("Recommendation with ID {Id} not found", id);
                return null;
            }

            var oldStatus = recommendation.Status;
            _mapper.Map(updateDto, recommendation);
            recommendation.UpdatedAt = DateTime.UtcNow;

            _dbContext.Recommendations.Update(recommendation);
            await _dbContext.SaveChangesAsync();

            // Publish event if status changed
            if (oldStatus != recommendation.Status)
            {
                try
                {
                    await _eventPublisher.PublishRecommendationStatusChangedEventAsync(recommendation, oldStatus);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to publish RecommendationStatusChangedEvent for recommendation {Id}", recommendation.Id);
                }
            }

            return _mapper.Map<RecommendationDto>(recommendation);
        }

        public async Task<RecommendationDto> ApproveRecommendationAsync(Guid id, ApproveRecommendationDto approveDto, Guid currentUserId, string currentUserName)
        {
            var recommendation = await _dbContext.Recommendations.FindAsync(id);
            if (recommendation == null)
            {
                _logger.LogWarning("Recommendation with ID {Id} not found", id);
                return null;
            }

            var oldStatus = recommendation.Status;
            recommendation.Status = RecommendationStatus.Approved;
            recommendation.ApprovedById = currentUserId;
            recommendation.ApprovedByName = currentUserName;
            recommendation.ApprovedAt = DateTime.UtcNow;
            recommendation.UpdatedAt = DateTime.UtcNow;
            recommendation.Notes = string.IsNullOrEmpty(approveDto.Notes)
                ? recommendation.Notes
                : approveDto.Notes;

            _dbContext.Recommendations.Update(recommendation);
            await _dbContext.SaveChangesAsync();

            // Publish events
            try
            {
                await _eventPublisher.PublishRecommendationApprovedEventAsync(recommendation);

                if (oldStatus != recommendation.Status)
                {
                    await _eventPublisher.PublishRecommendationStatusChangedEventAsync(recommendation, oldStatus);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish events for recommendation {Id}", recommendation.Id);
            }

            return _mapper.Map<RecommendationDto>(recommendation);
        }

        public async Task<RecommendationDto> UpdateRecommendationStatusAsync(Guid id, RecommendationStatus status)
        {
            var recommendation = await _dbContext.Recommendations.FindAsync(id);
            if (recommendation == null)
            {
                _logger.LogWarning("Recommendation with ID {Id} not found", id);
                return null;
            }

            var oldStatus = recommendation.Status;
            recommendation.Status = status;
            recommendation.UpdatedAt = DateTime.UtcNow;

            _dbContext.Recommendations.Update(recommendation);
            await _dbContext.SaveChangesAsync();

            // Publish event
            try
            {
                await _eventPublisher.PublishRecommendationStatusChangedEventAsync(recommendation, oldStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish RecommendationStatusChangedEvent for recommendation {Id}", recommendation.Id);
            }

            return _mapper.Map<RecommendationDto>(recommendation);
        }

        #endregion

        #region Recommendation Feedback

        public async Task<IEnumerable<RecommendationFeedbackDto>> GetFeedbackForRecommendationAsync(Guid recommendationId)
        {
            var feedback = await _dbContext.RecommendationFeedbacks
                .Where(f => f.RecommendationId == recommendationId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<RecommendationFeedbackDto>>(feedback);
        }

        public async Task<RecommendationFeedbackDto> GetFeedbackByIdAsync(Guid id)
        {
            var feedback = await _dbContext.RecommendationFeedbacks.FindAsync(id);
            if (feedback == null)
            {
                _logger.LogWarning("Recommendation feedback with ID {Id} not found", id);
                return null;
            }

            return _mapper.Map<RecommendationFeedbackDto>(feedback);
        }

        public async Task<RecommendationFeedbackDto> CreateFeedbackAsync(CreateRecommendationFeedbackDto createDto, Guid currentUserId, string currentUserName)
        {
            // Check if recommendation exists
            var recommendation = await _dbContext.Recommendations.FindAsync(createDto.RecommendationId);
            if (recommendation == null)
            {
                _logger.LogWarning("Recommendation with ID {Id} not found", createDto.RecommendationId);
                return null;
            }

            var feedback = _mapper.Map<RecommendationFeedback>(createDto);
            feedback.Id = Guid.NewGuid();
            feedback.ProvidedById = currentUserId;
            feedback.ProvidedByName = currentUserName;
            feedback.CreatedAt = DateTime.UtcNow;

            await _dbContext.RecommendationFeedbacks.AddAsync(feedback);
            await _dbContext.SaveChangesAsync();

            // Publish event
            try
            {
                await _eventPublisher.PublishRecommendationFeedbackAddedEventAsync(feedback, recommendation.PatientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish RecommendationFeedbackAddedEvent for feedback {Id}", feedback.Id);
            }

            return _mapper.Map<RecommendationFeedbackDto>(feedback);
        }

        #endregion

        #region Recommendation Templates

        public async Task<IEnumerable<RecommendationTemplateDto>> GetTemplatesAsync(bool includeInactive = false)
        {
            var query = _dbContext.RecommendationTemplates.AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(t => t.IsActive);
            }

            var templates = await query
                .OrderBy(t => t.Title)
                .ToListAsync();

            return _mapper.Map<IEnumerable<RecommendationTemplateDto>>(templates);
        }

        public async Task<RecommendationTemplateDto> GetTemplateByIdAsync(Guid id)
        {
            var template = await _dbContext.RecommendationTemplates.FindAsync(id);
            if (template == null)
            {
                _logger.LogWarning("Recommendation template with ID {Id} not found", id);
                return null;
            }

            return _mapper.Map<RecommendationTemplateDto>(template);
        }

        public async Task<RecommendationTemplateDto> CreateTemplateAsync(CreateRecommendationTemplateDto createDto, Guid currentUserId, string currentUserName)
        {
            var template = _mapper.Map<RecommendationTemplate>(createDto);
            template.Id = Guid.NewGuid();
            template.IsActive = true;
            template.CreatedById = currentUserId;
            template.CreatedByName = currentUserName;
            template.CreatedAt = DateTime.UtcNow;

            await _dbContext.RecommendationTemplates.AddAsync(template);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<RecommendationTemplateDto>(template);
        }

        public async Task<RecommendationTemplateDto> UpdateTemplateAsync(Guid id, UpdateRecommendationTemplateDto updateDto)
        {
            var template = await _dbContext.RecommendationTemplates.FindAsync(id);
            if (template == null)
            {
                _logger.LogWarning("Recommendation template with ID {Id} not found", id);
                return null;
            }

            _mapper.Map(updateDto, template);
            template.UpdatedAt = DateTime.UtcNow;

            _dbContext.RecommendationTemplates.Update(template);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<RecommendationTemplateDto>(template);
        }

        public async Task<bool> DeleteTemplateAsync(Guid id)
        {
            var template = await _dbContext.RecommendationTemplates.FindAsync(id);
            if (template == null)
            {
                _logger.LogWarning("Recommendation template with ID {Id} not found", id);
                return false;
            }

            // Soft delete by setting IsActive to false
            template.IsActive = false;
            template.UpdatedAt = DateTime.UtcNow;

            _dbContext.RecommendationTemplates.Update(template);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        #endregion
    }
}