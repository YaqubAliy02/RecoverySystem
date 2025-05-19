using RecoverySystem.BuildingBlocks.Events;
using RecoverySystem.BuildingBlocks.Events.Events;
using RecoverySystem.BuildingBlocks.Messaging.RabbitMQ;
using RecoverySystem.NotificationService.DTOs;
using RecoverySystem.NotificationService.Models.Enum;
using PatientUpdatedEvent = RecoverySystem.BuildingBlocks.Events.Events.PatientUpdatedEvent;

namespace RecoverySystem.NotificationService.Services
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
            _logger.LogInformation("Notification Service Event Consumer is starting");

            try
            {
                // Subscribe to patient events
                _consumer.Subscribe<PatientCreatedEvent>(
                    "recovery_system_exchange",
                    "notification_service_patient_created_queue",
                    "patient.created",
                    HandlePatientCreatedEvent);

                _consumer.Subscribe<PatientUpdatedEvent>(
                    "recovery_system_exchange",
                    "notification_service_patient_updated_queue",
                    "patient.updated",
                    HandlePatientUpdatedEvent);

                // Subscribe to case events
                _consumer.Subscribe<CaseCreatedEvent>(
                    "recovery_system_exchange",
                    "notification_service_case_created_queue",
                    "case.created",
                    HandleCaseCreatedEvent);

                _consumer.Subscribe<CaseStatusChangedEvent>(
                    "recovery_system_exchange",
                    "notification_service_case_status_changed_queue",
                    "case.status.changed",
                    HandleCaseStatusChangedEvent);

                // Subscribe to recommendation events
                _consumer.Subscribe<RecommendationCreatedEvent>(
                    "recovery_system_exchange",
                    "notification_service_recommendation_created_queue",
                    "recommendation.created",
                    HandleRecommendationCreatedEvent);

                // Subscribe to rehabilitation events
                _consumer.Subscribe<RehabilitationStartedEvent>(
                    "recovery_system_exchange",
                    "notification_service_rehabilitation_started_queue",
                    "rehabilitation.started",
                    HandleRehabilitationStartedEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting up event subscriptions");
            }

            return Task.CompletedTask;
        }

        private async Task HandlePatientCreatedEvent(PatientCreatedEvent @event)
        {
            _logger.LogInformation("Received PatientCreatedEvent for patient {PatientId}", @event.PatientId);

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                // Create notification for admin users (in a real system, you'd query for admin users)
                var createDto = new CreateNotificationDto
                {
                    Title = "New Patient Added",
                    Message = $"A new patient named {@event.Name} has been added to the system.",
                    Type = NotificationType.PatientUpdate,
                    TargetUserId = "admin", // In a real system, you'd send to specific users
                    SourceId = @event.PatientId,
                    SourceType = "Patient"
                };

                await notificationService.CreateNotificationAsync(createDto);
                _logger.LogInformation("Successfully created notification for PatientCreatedEvent");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PatientCreatedEvent for patient {PatientId}", @event.PatientId);
                throw;
            }
        }

        private async Task HandlePatientUpdatedEvent(PatientUpdatedEvent @event)
        {
            _logger.LogInformation("Received PatientUpdatedEvent for patient {PatientId}", @event.PatientId);

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                // Create notification for relevant users
                var createDto = new CreateNotificationDto
                {
                    Title = "Patient Information Updated",
                    Message = $"Patient {@event.Name} information has been updated.",
                    Type = NotificationType.PatientUpdate,
                    TargetUserId = "admin", // In a real system, you'd send to specific users
                    SourceId = @event.PatientId.ToString(),
                    SourceType = "Patient"
                };

                await notificationService.CreateNotificationAsync(createDto);
                _logger.LogInformation("Successfully created notification for PatientUpdatedEvent");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PatientUpdatedEvent for patient {PatientId}", @event.PatientId);
                throw;
            }
        }

        private async Task HandleCaseCreatedEvent(CaseCreatedEvent @event)
        {
            _logger.LogInformation("Received CaseCreatedEvent for case {CaseId}", @event.CaseId);

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                // Create notification for the assigned user
                var createDto = new CreateNotificationDto
                {
                    Title = "New Case Assigned",
                    Message = $"A new case '{@event.Title}' has been assigned to you.",
                    Type = NotificationType.CaseUpdate,
                    TargetUserId = @event.AssignedToId,
                    SourceId = @event.CaseId,
                    SourceType = "Case"
                };

                await notificationService.CreateNotificationAsync(createDto);
                _logger.LogInformation("Successfully created notification for CaseCreatedEvent");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing CaseCreatedEvent for case {CaseId}", @event.CaseId);
                throw;
            }
        }

        private async Task HandleCaseStatusChangedEvent(CaseStatusChangedEvent @event)
        {
            _logger.LogInformation("Received CaseStatusChangedEvent for case {CaseId}", @event.CaseId);

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                // Create notification for relevant users
                var createDto = new CreateNotificationDto
                {
                    Title = "Case Status Changed",
                    Message = $"Case status has changed from {@event.OldStatus} to {@event.NewStatus}.",
                    Type = NotificationType.CaseUpdate,
                    TargetUserId = "admin", // In a real system, you'd send to specific users
                    SourceId = @event.CaseId,
                    SourceType = "Case"
                };

                await notificationService.CreateNotificationAsync(createDto);
                _logger.LogInformation("Successfully created notification for CaseStatusChangedEvent");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing CaseStatusChangedEvent for case {CaseId}", @event.CaseId);
                throw;
            }
        }

        private async Task HandleRecommendationCreatedEvent(RecommendationCreatedEvent @event)
        {
            _logger.LogInformation("Received RecommendationCreatedEvent for recommendation {RecommendationId}", @event.RecommendationId);

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                // Create notification for relevant users
                var createDto = new CreateNotificationDto
                {
                    Title = "New Recommendation Added",
                    Message = $"A new recommendation '{@event.Title}' has been added for a patient.",
                    Type = NotificationType.RecommendationUpdate,
                    TargetUserId = "admin", // In a real system, you'd send to specific users
                    SourceId = @event.RecommendationId,
                    SourceType = "Recommendation"
                };

                await notificationService.CreateNotificationAsync(createDto);
                _logger.LogInformation("Successfully created notification for RecommendationCreatedEvent");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing RecommendationCreatedEvent for recommendation {RecommendationId}", @event.RecommendationId);
                throw;
            }
        }

        private async Task HandleRehabilitationStartedEvent(RehabilitationStartedEvent @event)
        {
            _logger.LogInformation("Received RehabilitationStartedEvent for rehabilitation {RehabilitationId}", @event.RehabilitationId);

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                // Create notification for the assigned user
                var createDto = new CreateNotificationDto
                {
                    Title = "New Rehabilitation Program Started",
                    Message = $"A new rehabilitation program '{@event.Title}' has been started.",
                    Type = NotificationType.RehabilitationUpdate,
                    TargetUserId = @event.AssignedToId,
                    SourceId = @event.RehabilitationId,
                    SourceType = "Rehabilitation"
                };

                await notificationService.CreateNotificationAsync(createDto);
                _logger.LogInformation("Successfully created notification for RehabilitationStartedEvent");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing RehabilitationStartedEvent for rehabilitation {RehabilitationId}", @event.RehabilitationId);
                throw;
            }
        }
    }
}