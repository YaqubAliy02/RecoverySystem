using RecoverySystem.BuildingBlocks.Events;
using RecoverySystem.BuildingBlocks.Messaging.RabbitMQ;
using PatientUpdatedEvent = RecoverySystem.BuildingBlocks.Events.Events.PatientUpdatedEvent;

namespace RecoverySystem.CaseService.Services
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
            _logger.LogInformation("Case Service Event Consumer is starting");

            // Subscribe to patient events
            _consumer.Subscribe<PatientUpdatedEvent>(
                "recovery_system_exchange",
                "case_service_patient_updated_queue",
                "patient.updated",
                HandlePatientUpdatedEvent);

            // Subscribe to user events
            _consumer.Subscribe<UserCreatedEvent>(
                "recovery_system_exchange",
                "case_service_user_created_queue",
                "user.created",
                HandleUserCreatedEvent);

            _consumer.Subscribe<UserUpdatedEvent>(
                "recovery_system_exchange",
                "case_service_user_updated_queue",
                "user.updated",
                HandleUserUpdatedEvent);

            return Task.CompletedTask;
        }

        private async Task HandlePatientUpdatedEvent(PatientUpdatedEvent @event)
        {
            _logger.LogInformation("Received PatientUpdatedEvent for patient {PatientId}", @event.PatientId);

            try
            {
                // Here you would update any patient information stored in the CaseService
                // For example, if you cache patient names in the Case entity
                using var scope = _serviceProvider.CreateScope();
                // var caseService = scope.ServiceProvider.GetRequiredService<ICaseService>();

                // Example: Update patient information in cases
                // await caseService.UpdatePatientInformationAsync(@event.PatientId, @event.Name);

                _logger.LogInformation("Successfully processed PatientUpdatedEvent for patient {PatientId}", @event.PatientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PatientUpdatedEvent for patient {PatientId}", @event.PatientId);
                throw; // Rethrowing will cause the message to be requeued
            }
        }

        private async Task HandleUserCreatedEvent(UserCreatedEvent @event)
        {
            _logger.LogInformation("Received UserCreatedEvent for user {UserId}", @event.UserId);

            try
            {
                // Here you would store user information if needed
                // For example, to maintain a local cache of users who can be assigned to cases

                _logger.LogInformation("Successfully processed UserCreatedEvent for user {UserId}", @event.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing UserCreatedEvent for user {UserId}", @event.UserId);
                throw;
            }
        }

        private async Task HandleUserUpdatedEvent(UserUpdatedEvent @event)
        {
            _logger.LogInformation("Received UserUpdatedEvent for user {UserId}", @event.UserId);

            try
            {
                // Here you would update user information if needed
                // For example, to update the name of users assigned to cases

                _logger.LogInformation("Successfully processed UserUpdatedEvent for user {UserId}", @event.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing UserUpdatedEvent for user {UserId}", @event.UserId);
                throw;
            }
        }
    }
}