using RecoverySystem.BuildingBlocks.Events.Events;
using RecoverySystem.BuildingBlocks.Messaging.RabbitMQ;
using RecoverySystem.NotificationService.Models;

namespace RecoverySystem.NotificationService.Services
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

        public async Task PublishNotificationCreatedEventAsync(Notification notification)
        {
            try
            {
                _logger.LogInformation("Publishing NotificationCreatedEvent for notification {NotificationId}", notification.Id);

                var @event = new NotificationCreatedEvent
                {
                    NotificationId = notification.Id.ToString(),
                    Title = notification.Title,
                    Message = notification.Message,
                    Type = notification.Type.ToString(),
                    TargetUserId = notification.TargetUserId,
                    CreatedAt = notification.CreatedAt
                };

                await _publisher.PublishAsync(
                    @event,
                    "recovery_system_exchange",
                    "notification.created");

                _logger.LogInformation("Successfully published NotificationCreatedEvent for notification {NotificationId}", notification.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish NotificationCreatedEvent for notification {NotificationId}", notification.Id);
            }
        }

        public async Task PublishNotificationReadEventAsync(Notification notification)
        {
            try
            {
                _logger.LogInformation("Publishing NotificationReadEvent for notification {NotificationId}", notification.Id);

                var @event = new NotificationReadEvent
                {
                    NotificationId = notification.Id.ToString(),
                    UserId = notification.TargetUserId,
                    ReadAt = notification.ReadAt ?? DateTime.UtcNow
                };

                await _publisher.PublishAsync(
                    @event,
                    "recovery_system_exchange",
                    "notification.read");

                _logger.LogInformation("Successfully published NotificationReadEvent for notification {NotificationId}", notification.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish NotificationReadEvent for notification {NotificationId}", notification.Id);
            }
        }
    }
}