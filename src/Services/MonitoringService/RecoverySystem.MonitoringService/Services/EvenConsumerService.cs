using RecoverySystem.BuildingBlocks.Events;
using RecoverySystem.BuildingBlocks.Events.Events;
using RecoverySystem.BuildingBlocks.Messaging.RabbitMQ;
using RecoverySystem.MonitoringService.DTOs;
using PatientUpdatedEvent = RecoverySystem.BuildingBlocks.Events.Events.PatientUpdatedEvent;

namespace RecoverySystem.MonitoringService.Services
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
            _logger.LogInformation("Monitoring Service Event Consumer is starting");

            try
            {
                // Subscribe to patient events
                _consumer.Subscribe<PatientCreatedEvent>(
                    "recovery_system_exchange",
                    "monitoring_service_patient_created_queue",
                    "patient.created",
                    HandlePatientCreatedEvent);

                _consumer.Subscribe<PatientUpdatedEvent>(
                    "recovery_system_exchange",
                    "monitoring_service_patient_updated_queue",
                    "patient.updated",
                    HandlePatientUpdatedEvent);

                // Subscribe to case events
                _consumer.Subscribe<CaseCreatedEvent>(
                    "recovery_system_exchange",
                    "monitoring_service_case_created_queue",
                    "case.created",
                    HandleCaseCreatedEvent);

                _consumer.Subscribe<CaseStatusChangedEvent>(
                    "recovery_system_exchange",
                    "monitoring_service_case_status_changed_queue",
                    "case.status.changed",
                    HandleCaseStatusChangedEvent);

                // Subscribe to rehabilitation events
                _consumer.Subscribe<RehabilitationStartedEvent>(
                    "recovery_system_exchange",
                    "monitoring_service_rehabilitation_started_queue",
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

            // Defensive: check PatientId
            if (string.IsNullOrWhiteSpace(@event.PatientId) || !Guid.TryParse(@event.PatientId, out var patientGuid))
            {
                _logger.LogError("Invalid or missing PatientId in PatientCreatedEvent: '{PatientId}'", @event.PatientId);
                return;
            }

            try
            {
                // Create default threshold configurations for the new patient
                using var scope = _serviceProvider.CreateScope();
                var monitoringService = scope.ServiceProvider.GetRequiredService<IMonitoringService>();

                // Heart rate thresholds (60-100 bpm)
                await monitoringService.CreateThresholdConfigurationAsync(new CreateThresholdConfigurationDto
                {
                    Name = "Heart Rate - Normal Range",
                    VitalSign = "HeartRate",
                    LowerThreshold = 60,
                    UpperThreshold = 100,
                    Severity = Models.AlertSeverity.Medium,
                    IsGlobal = false,
                    PatientId = patientGuid
                });

                // Temperature thresholds (36.1-37.2 °C)
                await monitoringService.CreateThresholdConfigurationAsync(new CreateThresholdConfigurationDto
                {
                    Name = "Temperature - Normal Range",
                    VitalSign = "Temperature",
                    LowerThreshold = 36.1,
                    UpperThreshold = 37.2,
                    Severity = Models.AlertSeverity.Medium,
                    IsGlobal = false,
                    PatientId = patientGuid
                });

                // Respiratory rate thresholds (12-20 breaths per minute)
                await monitoringService.CreateThresholdConfigurationAsync(new CreateThresholdConfigurationDto
                {
                    Name = "Respiratory Rate - Normal Range",
                    VitalSign = "RespiratoryRate",
                    LowerThreshold = 12,
                    UpperThreshold = 20,
                    Severity = Models.AlertSeverity.Medium,
                    IsGlobal = false,
                    PatientId = patientGuid
                });

                // Oxygen saturation thresholds (95-100%)
                await monitoringService.CreateThresholdConfigurationAsync(new CreateThresholdConfigurationDto
                {
                    Name = "Oxygen Saturation - Normal Range",
                    VitalSign = "OxygenSaturation",
                    LowerThreshold = 95,
                    UpperThreshold = 100,
                    Severity = Models.AlertSeverity.High,
                    IsGlobal = false,
                    PatientId = patientGuid
                });

                // Pain level thresholds (0-3 out of 10)
                await monitoringService.CreateThresholdConfigurationAsync(new CreateThresholdConfigurationDto
                {
                    Name = "Pain Level - Normal Range",
                    VitalSign = "PainLevel",
                    LowerThreshold = 0,
                    UpperThreshold = 3,
                    Severity = Models.AlertSeverity.Low,
                    IsGlobal = false,
                    PatientId = patientGuid
                });

                _logger.LogInformation("Successfully created default threshold configurations for patient {PatientId}", @event.PatientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PatientCreatedEvent for patient {PatientId}", @event.PatientId);
                throw;
            }
        }

        private Task HandlePatientUpdatedEvent(PatientUpdatedEvent @event)
        {
            _logger.LogInformation("Received PatientUpdatedEvent for patient {PatientId}", @event.PatientId);
            // No specific action needed for patient updates in the monitoring service
            return Task.CompletedTask;
        }

        private Task HandleCaseCreatedEvent(CaseCreatedEvent @event)
        {
            _logger.LogInformation("Received CaseCreatedEvent for case {CaseId}", @event.CaseId);
            // No specific action needed for case creation in the monitoring service
            return Task.CompletedTask;
        }

        private Task HandleCaseStatusChangedEvent(CaseStatusChangedEvent @event)
        {
            _logger.LogInformation("Received CaseStatusChangedEvent for case {CaseId}", @event.CaseId);
            // No specific action needed for case status changes in the monitoring service
            return Task.CompletedTask;
        }

        private Task HandleRehabilitationStartedEvent(RehabilitationStartedEvent @event)
        {
            _logger.LogInformation("Received RehabilitationStartedEvent for rehabilitation {RehabilitationId}", @event.RehabilitationId);
            // No specific action needed for rehabilitation starts in the monitoring service
            return Task.CompletedTask;
        }
    }
}