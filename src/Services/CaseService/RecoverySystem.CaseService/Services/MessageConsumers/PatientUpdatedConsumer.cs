using RecoverySystem.BuildingBlocks.Events;
using RecoverySystem.BuildingBlocks.Messaging.RabbitMQ;

namespace RecoverySystem.CaseService.Services.MessageConsumers
{
    public class PatientUpdatedConsumer : BackgroundService
    {
        private readonly IRabbitMQConsumer _consumer;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PatientUpdatedConsumer> _logger;
        private const string ExchangeName = "recovery_system_exchange";
        private const string QueueName = "case-service.patient-updated";
        private const string RoutingKey = "patient.updated";

        public PatientUpdatedConsumer(
            IRabbitMQConsumer consumer,
            IServiceProvider serviceProvider,
            ILogger<PatientUpdatedConsumer> logger)
        {
            _consumer = consumer ?? throw new ArgumentNullException(nameof(consumer));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting PatientUpdatedConsumer");

            _consumer.Subscribe<PatientUpdatedEvent>(
                ExchangeName,
                QueueName,
                RoutingKey,
                HandlePatientUpdatedEvent);

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
    }
}