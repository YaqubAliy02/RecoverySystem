using RecoverySystem.BuildingBlocks.Events.Events;
using RecoverySystem.BuildingBlocks.Messaging.RabbitMQ;
using RecoverySystem.RecommendationService.Models;

namespace RecoverySystem.RecommendationService.Services
{
    public class EventPublisher
    {
        private readonly IRabbitMQPublisher _publisher;
        private readonly ILogger<EventPublisher> _logger;

        public EventPublisher(IRabbitMQPublisher publisher, ILogger<EventPublisher> logger)
        {
            _publisher = publisher;
            _logger = logger;
        }

        public async Task PublishRecommendationCreatedEventAsync(Recommendation recommendation)
        {
            try
            {
                _logger.LogInformation("Publishing RecommendationCreatedEvent for recommendation {Id}", recommendation.Id);

                var @event = new RecommendationCreatedEvent
                {
                    RecommendationId = recommendation.Id.ToString(),
                    Title = recommendation.Title,
                    Description = recommendation.Description,
                    Type = recommendation.Type.ToString(),
                    PatientId = recommendation.PatientId.ToString(),
                    PatientName = recommendation.PatientName,
                    CaseId = recommendation.CaseId?.ToString(),
                    Priority = recommendation.Priority.ToString(),
                    StartDate = recommendation.StartDate,
                    EndDate = recommendation.EndDate,
                    Tags = recommendation.Tags,
                    CreatedById = recommendation.CreatedById.ToString(),
                    CreatedByName = recommendation.CreatedByName,
                    CreatedAt = recommendation.CreatedAt
                };

                await _publisher.PublishAsync(
                    @event,
                    "recovery_system_exchange",
                    "recommendation.created");

                _logger.LogInformation("Successfully published RecommendationCreatedEvent for recommendation {Id}", recommendation.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish RecommendationCreatedEvent for recommendation {Id}", recommendation.Id);
            }
        }

        public async Task PublishRecommendationApprovedEventAsync(Recommendation recommendation)
        {
            try
            {
                _logger.LogInformation("Publishing RecommendationApprovedEvent for recommendation {Id}", recommendation.Id);

                var @event = new RecommendationApprovedEvent
                {
                    RecommendationId = recommendation.Id.ToString(),
                    PatientId = recommendation.PatientId.ToString(),
                    ApprovedById = recommendation.ApprovedById.ToString(),
                    ApprovedByName = recommendation.ApprovedByName,
                    ApprovedAt = recommendation.ApprovedAt ?? DateTime.UtcNow
                };

                await _publisher.PublishAsync(
                    @event,
                    "recovery_system_exchange",
                    "recommendation.approved");

                _logger.LogInformation("Successfully published RecommendationApprovedEvent for recommendation {Id}", recommendation.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish RecommendationApprovedEvent for recommendation {Id}", recommendation.Id);
            }
        }

        public async Task PublishRecommendationStatusChangedEventAsync(Recommendation recommendation, RecommendationStatus oldStatus)
        {
            try
            {
                _logger.LogInformation("Publishing RecommendationStatusChangedEvent for recommendation {Id}", recommendation.Id);

                var @event = new RecommendationStatusChangedEvent
                {
                    RecommendationId = recommendation.Id.ToString(),
                    PatientId = recommendation.PatientId.ToString(),
                    OldStatus = oldStatus.ToString(),
                    NewStatus = recommendation.Status.ToString(),
                    UpdatedAt = recommendation.UpdatedAt ?? DateTime.UtcNow
                };

                await _publisher.PublishAsync(
                    @event,
                    "recovery_system_exchange",
                    "recommendation.status.changed");

                _logger.LogInformation("Successfully published RecommendationStatusChangedEvent for recommendation {Id}", recommendation.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish RecommendationStatusChangedEvent for recommendation {Id}", recommendation.Id);
            }
        }

        public async Task PublishRecommendationFeedbackAddedEventAsync(RecommendationFeedback feedback, Guid patientId)
        {
            try
            {
                _logger.LogInformation("Publishing RecommendationFeedbackAddedEvent for feedback {Id}", feedback.Id);

                var @event = new RecommendationFeedbackAddedEvent
                {
                    FeedbackId = feedback.Id.ToString(),
                    RecommendationId = feedback.RecommendationId.ToString(),
                    PatientId = patientId.ToString(),
                    Rating = feedback.Rating,
                    IsEffective = feedback.IsEffective,
                    IsPatientFeedback = feedback.IsPatientFeedback,
                    CreatedAt = feedback.CreatedAt
                };

                await _publisher.PublishAsync(
                    @event,
                    "recovery_system_exchange",
                    "recommendation.feedback.added");

                _logger.LogInformation("Successfully published RecommendationFeedbackAddedEvent for feedback {Id}", feedback.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish RecommendationFeedbackAddedEvent for feedback {Id}", feedback.Id);
            }
        }
    }
}