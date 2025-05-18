using RecoverySystem.BuildingBlocks.Events;
using RecoverySystem.BuildingBlocks.Messaging.RabbitMQ;

namespace RecoverySystem.PatientService.Services;

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
        _logger.LogInformation("Patient Service Event Consumer is starting");

        // Subscribe to user events
        _consumer.Subscribe<UserCreatedEvent>(
            "recovery_system_exchange",
            "patient_service_user_created_queue",
            "user.created",
            async @event =>
            {
                _logger.LogInformation("Received UserCreatedEvent for user {UserId}", @event.UserId);
                // Process user created event if needed
                // For example, you might want to store user information for reference
            });

        return Task.CompletedTask;
    }
}