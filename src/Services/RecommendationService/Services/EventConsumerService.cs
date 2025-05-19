using RecoverySystem.BuildingBlocks.Events;
using RecoverySystem.BuildingBlocks.Events.Events;
using RecoverySystem.BuildingBlocks.Messaging.RabbitMQ;
using RecoverySystem.RecommendationService.DTOs;
using RecoverySystem.RecommendationService.Models;
using PatientUpdatedEvent = RecoverySystem.BuildingBlocks.Events.Events.PatientUpdatedEvent;

namespace RecoverySystem.RecommendationService.Services
{
    public class EventConsumerService : BackgroundService
    {
        private readonly IRabbitMQConsumer _consumer;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EventConsumerService> _logger;

        public EventConsumerService(
            IRabbitMQConsumer consumer,
            IServiceProvider serviceProvider,
            ILogger<EventConsumerService> logger)
        {
            _consumer = consumer;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Recommendation Service Event Consumer is starting");

            try
            {
                // Subscribe to patient events
                _consumer.Subscribe<PatientCreatedEvent>(
                    "recovery_system_exchange",
                    "recommendation_service_patient_created_queue",
                    "patient.created",
                    HandlePatientCreatedEvent);

                _consumer.Subscribe<PatientUpdatedEvent>(
                    "recovery_system_exchange",
                    "recommendation_service_patient_updated_queue",
                    "patient.updated",
                    HandlePatientUpdatedEvent);

                // Subscribe to case events
                _consumer.Subscribe<CaseCreatedEvent>(
                    "recovery_system_exchange",
                    "recommendation_service_case_created_queue",
                    "case.created",
                    HandleCaseCreatedEvent);

                _consumer.Subscribe<CaseStatusChangedEvent>(
                    "recovery_system_exchange",
                    "recommendation_service_case_status_changed_queue",
                    "case.status.changed",
                    HandleCaseStatusChangedEvent);

                // Subscribe to monitoring events
                _consumer.Subscribe<VitalSignsRecordedEvent>(
                    "recovery_system_exchange",
                    "recommendation_service_vital_signs_recorded_queue",
                    "monitoring.vitalsigns.recorded",
                    HandleVitalSignsRecordedEvent);

                _consumer.Subscribe<AlertCreatedEvent>(
                    "recovery_system_exchange",
                    "recommendation_service_alert_created_queue",
                    "monitoring.alert.created",
                    HandleAlertCreatedEvent);

                // Subscribe to rehabilitation events
                _consumer.Subscribe<RehabilitationSessionCompletedEvent>(
                    "recovery_system_exchange",
                    "recommendation_service_rehabilitation_session_completed_queue",
                    "rehabilitation.session.completed",
                    HandleRehabilitationSessionCompletedEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting up event subscriptions");
            }

            return Task.CompletedTask;
        }

        private Task HandlePatientCreatedEvent(PatientCreatedEvent @event)
        {
            _logger.LogInformation("Received PatientCreatedEvent for patient {PatientId}", @event.PatientId);
            // No specific action needed for patient creation in the recommendation service
            return Task.CompletedTask;
        }

        private Task HandlePatientUpdatedEvent(PatientUpdatedEvent @event)
        {
            _logger.LogInformation("Received PatientUpdatedEvent for patient {PatientId}", @event.PatientId);
            // No specific action needed for patient updates in the recommendation service
            return Task.CompletedTask;
        }

        private Task HandleCaseCreatedEvent(CaseCreatedEvent @event)
        {
            _logger.LogInformation("Received CaseCreatedEvent for case {CaseId}", @event.CaseId);
            // No specific action needed for case creation in the recommendation service
            return Task.CompletedTask;
        }

        private async Task HandleCaseStatusChangedEvent(CaseStatusChangedEvent @event)
        {
            _logger.LogInformation("Received CaseStatusChangedEvent for case {CaseId}", @event.CaseId);

            // If case is closed, complete any active recommendations for this case
            if (@event.NewStatus.ToLower() == "closed")
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var recommendationService = scope.ServiceProvider.GetRequiredService<IRecommendationService>();

                    var recommendations = await recommendationService.GetRecommendationsForCaseAsync(Guid.Parse(@event.CaseId));
                    foreach (var recommendation in recommendations)
                    {
                        if (recommendation.Status == RecommendationStatus.InProgress ||
                            recommendation.Status == RecommendationStatus.Pending ||
                            recommendation.Status == RecommendationStatus.Approved)
                        {
                            await recommendationService.UpdateRecommendationStatusAsync(
                                recommendation.Id,
                                RecommendationStatus.Completed);

                            _logger.LogInformation("Completed recommendation {RecommendationId} due to case closure", recommendation.Id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing CaseStatusChangedEvent for case {CaseId}", @event.CaseId);
                    throw;
                }
            }
        }

        private async Task HandleVitalSignsRecordedEvent(VitalSignsRecordedEvent @event)
        {
            _logger.LogInformation("Received VitalSignsRecordedEvent for patient {PatientId}", @event.PatientId);

            // Check if vital signs indicate a need for recommendations
            try
            {
                // Example: If heart rate is too high, create an exercise recommendation
                if (@event.HeartRate > 100)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var recommendationService = scope.ServiceProvider.GetRequiredService<IRecommendationService>();

                    var createDto = new CreateRecommendationDto
                    {
                        Title = "Recommended: Cardiovascular Exercise Reduction",
                        Description = "Based on recent vital signs monitoring, we recommend reducing cardiovascular exercise intensity.",
                        Type = RecommendationType.Exercise,
                        PatientId = Guid.Parse(@event.PatientId),
                        PatientName = "Patient", // In a real system, you'd get the patient name
                        Priority = RecommendationPriority.Medium,
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddDays(14),
                        Instructions = "Reduce cardiovascular exercise intensity by 30%. Focus on low-impact activities like walking and swimming.",
                        Tags = new List<string> { "heart-rate", "exercise", "cardiovascular" },
                        Notes = $"Automatically generated based on vital signs monitoring. Heart rate recorded: {@event.HeartRate} bpm."
                    };

                    await recommendationService.CreateRecommendationAsync(
                        createDto,
                        Guid.NewGuid(), // System user ID
                        "System");

                    _logger.LogInformation("Created exercise recommendation for patient {PatientId} due to high heart rate", @event.PatientId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing VitalSignsRecordedEvent for patient {PatientId}", @event.PatientId);
                throw;
            }
        }

        private async Task HandleAlertCreatedEvent(AlertCreatedEvent @event)
        {
            _logger.LogInformation("Received AlertCreatedEvent for patient {PatientId}", @event.PatientId);

            // If alert is critical, create a recommendation
            if (@event.Severity.ToLower() == "critical" || @event.Severity.ToLower() == "high")
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var recommendationService = scope.ServiceProvider.GetRequiredService<IRecommendationService>();

                    var createDto = new CreateRecommendationDto
                    {
                        Title = $"Urgent: Response to {(@event.Severity.ToLower() == "critical" ? "Critical" : "High")} Alert",
                        Description = $"This recommendation is in response to a {(@event.Severity.ToLower() == "critical" ? "critical" : "high")} alert: {@event.Message}",
                        Type = RecommendationType.Other,
                        PatientId = Guid.Parse(@event.PatientId),
                        PatientName = @event.PatientName,
                        Priority = @event.Severity.ToLower() == "critical" ? RecommendationPriority.Critical : RecommendationPriority.High,
                        StartDate = DateTime.UtcNow,
                        Instructions = $"Please review the patient's condition immediately. Alert details: {@event.Message}",
                        Tags = new List<string> { "alert", @event.Type.ToLower(), @event.Severity.ToLower() },
                        Notes = $"Automatically generated based on alert: {@event.AlertId}"
                    };

                    await recommendationService.CreateRecommendationAsync(
                        createDto,
                        Guid.NewGuid(), // System user ID
                        "System");

                    _logger.LogInformation("Created urgent recommendation for patient {PatientId} due to {Severity} alert",
                        @event.PatientId, @event.Severity);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing AlertCreatedEvent for patient {PatientId}", @event.PatientId);
                    throw;
                }
            }
        }

        private async Task HandleRehabilitationSessionCompletedEvent(RehabilitationSessionCompletedEvent @event)
        {
            _logger.LogInformation("Received RehabilitationSessionCompletedEvent for session {SessionId}", @event.SessionId);

            // If pain level is high, create a recommendation
            if (@event.PainLevel > 7)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var recommendationService = scope.ServiceProvider.GetRequiredService<IRecommendationService>();

                    var createDto = new CreateRecommendationDto
                    {
                        Title = "Pain Management Recommendation",
                        Description = "Based on high pain levels reported during rehabilitation, we recommend a pain management consultation.",
                        Type = RecommendationType.Therapy,
                        PatientId = Guid.Parse(@event.PatientId),
                        PatientName = "Patient", // In a real system, you'd get the patient name
                        Priority = RecommendationPriority.High,
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddDays(7),
                        Instructions = "Schedule a pain management consultation within the next 48 hours. Consider temporary reduction in rehabilitation intensity.",
                        Tags = new List<string> { "pain", "rehabilitation", "therapy" },
                        Notes = $"Automatically generated based on high pain level ({@event.PainLevel}/10) reported during rehabilitation session."
                    };

                    await recommendationService.CreateRecommendationAsync(
                        createDto,
                        Guid.NewGuid(), // System user ID
                        "System");

                    _logger.LogInformation("Created pain management recommendation for patient {PatientId} due to high pain level", @event.PatientId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing RehabilitationSessionCompletedEvent for session {SessionId}", @event.SessionId);
                    throw;
                }
            }
            // If fatigue level is high, create a recommendation
            else if (@event.FatigueLevel > 8)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var recommendationService = scope.ServiceProvider.GetRequiredService<IRecommendationService>();

                    var createDto = new CreateRecommendationDto
                    {
                        Title = "Fatigue Management Recommendation",
                        Description = "Based on high fatigue levels reported during rehabilitation, we recommend adjusting the rehabilitation schedule.",
                        Type = RecommendationType.Lifestyle,
                        PatientId = Guid.Parse(@event.PatientId),
                        PatientName = "Patient", // In a real system, you'd get the patient name
                        Priority = RecommendationPriority.Medium,
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddDays(14),
                        Instructions = "Reduce rehabilitation session frequency by 30%. Ensure adequate rest periods between sessions. Consider nutritional consultation.",
                        Tags = new List<string> { "fatigue", "rehabilitation", "lifestyle" },
                        Notes = $"Automatically generated based on high fatigue level ({@event.FatigueLevel}/10) reported during rehabilitation session."
                    };

                    await recommendationService.CreateRecommendationAsync(
                        createDto,
                        Guid.NewGuid(), // System user ID
                        "System");

                    _logger.LogInformation("Created fatigue management recommendation for patient {PatientId} due to high fatigue level", @event.PatientId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing RehabilitationSessionCompletedEvent for session {SessionId}", @event.SessionId);
                    throw;
                }
            }
        }
    }
}