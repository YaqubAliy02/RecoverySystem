using RecoverySystem.BuildingBlocks.Events.Events;
using RecoverySystem.BuildingBlocks.Messaging.RabbitMQ;
using RecoverySystem.MonitoringService.Models;

namespace RecoverySystem.MonitoringService.Services
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

        public async Task PublishVitalSignsRecordedEventAsync(VitalMonitoring vitalMonitoring)
        {
            try
            {
                _logger.LogInformation("Publishing VitalSignsRecordedEvent for monitoring {Id}", vitalMonitoring.Id);

                var @event = new VitalSignsRecordedEvent
                {
                    MonitoringId = vitalMonitoring.Id.ToString(),
                    PatientId = vitalMonitoring.PatientId.ToString(),
                    HeartRate = vitalMonitoring.HeartRate,
                    BloodPressure = vitalMonitoring.BloodPressure,
                    Temperature = vitalMonitoring.Temperature,
                    RespiratoryRate = vitalMonitoring.RespiratoryRate,
                    OxygenSaturation = vitalMonitoring.OxygenSaturation,
                    PainLevel = vitalMonitoring.PainLevel,
                    Timestamp = vitalMonitoring.Timestamp
                };

                await _publisher.PublishAsync(
                    @event,
                    "recovery_system_exchange",
                    "monitoring.vitalsigns.recorded");

                _logger.LogInformation("Successfully published VitalSignsRecordedEvent for monitoring {Id}", vitalMonitoring.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish VitalSignsRecordedEvent for monitoring {Id}", vitalMonitoring.Id);
            }
        }

        public async Task PublishAlertCreatedEventAsync(Alert alert)
        {
            try
            {
                _logger.LogInformation("Publishing AlertCreatedEvent for alert {Id}", alert.Id);

                var @event = new AlertCreatedEvent
                {
                    AlertId = alert.Id.ToString(),
                    PatientId = alert.PatientId.ToString(),
                    PatientName = alert.PatientName,
                    Type = alert.Type.ToString(),
                    Severity = alert.Severity.ToString(),
                    Message = alert.Message,
                    CreatedAt = alert.CreatedAt
                };

                await _publisher.PublishAsync(
                    @event,
                    "recovery_system_exchange",
                    "monitoring.alert.created");

                _logger.LogInformation("Successfully published AlertCreatedEvent for alert {Id}", alert.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish AlertCreatedEvent for alert {Id}", alert.Id);
            }
        }

        public async Task PublishAlertResolvedEventAsync(Alert alert)
        {
            try
            {
                _logger.LogInformation("Publishing AlertResolvedEvent for alert {Id}", alert.Id);

                var @event = new AlertResolvedEvent
                {
                    AlertId = alert.Id.ToString(),
                    PatientId = alert.PatientId.ToString(),
                    ResolvedById = alert.ResolvedById.ToString(),
                    ResolvedByName = alert.ResolvedByName,
                    ResolvedAt = alert.ResolvedAt ?? DateTime.UtcNow
                };

                await _publisher.PublishAsync(
                    @event,
                    "recovery_system_exchange",
                    "monitoring.alert.resolved");

                _logger.LogInformation("Successfully published AlertResolvedEvent for alert {Id}", alert.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish AlertResolvedEvent for alert {Id}", alert.Id);
            }
        }

        public async Task PublishSystemHealthChangedEventAsync(SystemHealth systemHealth)
        {
            try
            {
                _logger.LogInformation("Publishing SystemHealthChangedEvent for service {ServiceName}", systemHealth.ServiceName);

                var @event = new SystemHealthChangedEvent
                {
                    ServiceName = systemHealth.ServiceName,
                    Status = systemHealth.Status,
                    Description = systemHealth.Description,
                    Timestamp = systemHealth.Timestamp
                };

                await _publisher.PublishAsync(
                    @event,
                    "recovery_system_exchange",
                    "monitoring.systemhealth.changed");

                _logger.LogInformation("Successfully published SystemHealthChangedEvent for service {ServiceName}", systemHealth.ServiceName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish SystemHealthChangedEvent for service {ServiceName}", systemHealth.ServiceName);
            }
        }
    }
}