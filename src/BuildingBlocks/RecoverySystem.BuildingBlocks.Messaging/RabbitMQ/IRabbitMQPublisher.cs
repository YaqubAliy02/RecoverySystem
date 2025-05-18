using RecoverySystem.BuildingBlocks.Messaging.Messages;

namespace RecoverySystem.BuildingBlocks.Messaging.RabbitMQ;

public interface IRabbitMQPublisher
{
    Task PublishAsync<T>(T @event, string exchangeName, string routingKey, CancellationToken cancellationToken = default)
        where T : IntegrationEvent;
}