// RabbitMQ/IRabbitMQConsumer.cs
using RecoverySystem.BuildingBlocks.Messaging.Messages;

namespace RecoverySystem.BuildingBlocks.Messaging.RabbitMQ;

public interface IRabbitMQConsumer
{
    void Subscribe<T>(string exchangeName, string queueName, string routingKey, Func<T, Task> handler)
        where T : IntegrationEvent;
}