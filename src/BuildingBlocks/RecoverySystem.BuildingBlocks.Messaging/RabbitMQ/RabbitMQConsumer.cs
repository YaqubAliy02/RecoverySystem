using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RecoverySystem.BuildingBlocks.Messaging.Configuration;
using RecoverySystem.BuildingBlocks.Messaging.Messages;

namespace RecoverySystem.BuildingBlocks.Messaging.RabbitMQ;

public class RabbitMQConsumer : IRabbitMQConsumer, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMQConsumer> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly Dictionary<string, IModel> _consumerChannels;
    private bool _disposed;

    public RabbitMQConsumer(
        IOptions<RabbitMQSettings> options,
        ILogger<RabbitMQConsumer> logger)
    {
        _logger = logger;
        _consumerChannels = new Dictionary<string, IModel>();

        var settings = options.Value;
        var factory = new ConnectionFactory
        {
            HostName = settings.HostName,
            Port = settings.Port,
            UserName = settings.UserName,
            Password = settings.Password,
            VirtualHost = settings.VirtualHost,
            AutomaticRecoveryEnabled = settings.AutomaticRecoveryEnabled,
            NetworkRecoveryInterval = settings.NetworkRecoveryInterval,
            DispatchConsumersAsync = true // Enable async consumers
        };

        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _logger.LogInformation("RabbitMQ connection established for consumer");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not connect to RabbitMQ for consumer");
            throw;
        }

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public void Subscribe<T>(string exchangeName, string queueName, string routingKey, Func<T, Task> handler)
        where T : IntegrationEvent
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(RabbitMQConsumer));

        // Create a dedicated channel for this consumer
        var consumerChannel = _connection.CreateModel();
        _consumerChannels[queueName] = consumerChannel;

        _logger.LogInformation("Setting up consumer for queue {QueueName} with routing key {RoutingKey} on exchange {ExchangeName}",
            queueName, routingKey, exchangeName);

        // Declare exchange
        consumerChannel.ExchangeDeclare(
            exchange: exchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false);

        // Declare queue
        consumerChannel.QueueDeclare(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        // Bind queue to exchange
        consumerChannel.QueueBind(
            queue: queueName,
            exchange: exchangeName,
            routingKey: routingKey);

        // Set prefetch count
        consumerChannel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        // Create consumer
        var consumer = new AsyncEventingBasicConsumer(consumerChannel);

        consumer.Received += async (sender, args) =>
        {
            var messageId = args.BasicProperties.MessageId;
            _logger.LogInformation("Processing message {MessageId} from queue {QueueName}", messageId, queueName);

            try
            {
                var message = Encoding.UTF8.GetString(args.Body.Span);
                var @event = JsonSerializer.Deserialize<T>(message, _jsonOptions);

                if (@event != null)
                {
                    await handler(@event);
                    consumerChannel.BasicAck(args.DeliveryTag, multiple: false);
                    _logger.LogInformation("Message {MessageId} processed successfully", messageId);
                }
                else
                {
                    _logger.LogWarning("Message {MessageId} could not be deserialized", messageId);
                    consumerChannel.BasicNack(args.DeliveryTag, multiple: false, requeue: false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message {MessageId}", messageId);
                consumerChannel.BasicNack(args.DeliveryTag, multiple: false, requeue: true);
            }
        };

        // Start consuming
        consumerChannel.BasicConsume(
            queue: queueName,
            autoAck: false,
            consumer: consumer);

        _logger.LogInformation("Consumer for queue {QueueName} started", queueName);
    }

    public void Dispose()
    {
        if (_disposed) return;

        foreach (var channel in _consumerChannels.Values)
        {
            if (channel?.IsOpen == true)
            {
                channel.Close();
                channel.Dispose();
            }
        }

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