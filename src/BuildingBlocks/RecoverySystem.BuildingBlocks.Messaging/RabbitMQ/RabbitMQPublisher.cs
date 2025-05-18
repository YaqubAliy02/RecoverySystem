using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RecoverySystem.BuildingBlocks.Messaging.Configuration;
using RecoverySystem.BuildingBlocks.Messaging.Messages;

namespace RecoverySystem.BuildingBlocks.Messaging.RabbitMQ;

public class RabbitMQPublisher : IRabbitMQPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMQPublisher> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private bool _disposed;

    public RabbitMQPublisher(
        IOptions<RabbitMQSettings> options,
        ILogger<RabbitMQPublisher> logger)
    {
        _logger = logger;

        var settings = options.Value;
        var factory = new ConnectionFactory
        {
            HostName = settings.HostName,
            Port = settings.Port,
            UserName = settings.UserName,
            Password = settings.Password,
            VirtualHost = settings.VirtualHost,
            AutomaticRecoveryEnabled = settings.AutomaticRecoveryEnabled,
            NetworkRecoveryInterval = settings.NetworkRecoveryInterval
        };

        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _logger.LogInformation("RabbitMQ connection established");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not connect to RabbitMQ");
            throw;
        }

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    public Task PublishAsync<T>(T @event, string exchangeName, string routingKey, CancellationToken cancellationToken = default)
        where T : IntegrationEvent
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(RabbitMQPublisher));

        _logger.LogInformation("Publishing event {EventId} to {Exchange} with routing key {RoutingKey}",
            @event.Id, exchangeName, routingKey);

        // Ensure exchange exists
        _channel.ExchangeDeclare(
            exchange: exchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false);

        // Serialize the message
        var message = JsonSerializer.Serialize(@event, @event.GetType(), _jsonOptions);
        var body = Encoding.UTF8.GetBytes(message);

        // Create message properties
        var properties = _channel.CreateBasicProperties();
        properties.DeliveryMode = 2; // persistent
        properties.MessageId = @event.Id.ToString();
        properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        properties.ContentType = "application/json";
        properties.Type = @event.GetType().Name;

        // Publish the message
        _channel.BasicPublish(
            exchange: exchangeName,
            routingKey: routingKey,
            mandatory: true,
            basicProperties: properties,
            body: body);

        _logger.LogInformation("Event {EventId} published successfully", @event.Id);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_disposed) return;

        if (_channel?.IsOpen == true)
        {
            _channel.Close();
            _channel.Dispose();
        }

        if (_connection?.IsOpen == true)
        {
            _connection.Close();
            _connection.Dispose();
        }

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}