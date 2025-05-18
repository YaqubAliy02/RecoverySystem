using RecoverySystem.BuildingBlocks.Events;
using RecoverySystem.BuildingBlocks.Messaging.RabbitMQ;
using RecoverySystem.IdentityService.Models;

namespace RecoverySystem.IdentityService.Services;

public class EventPublisher
{
    private readonly IRabbitMQPublisher _publisher;
    private readonly ILogger<EventPublisher> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EventPublisher(
        IRabbitMQPublisher publisher,
        ILogger<EventPublisher> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _publisher = publisher;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task PublishUserCreatedEventAsync(ApplicationUser user)
    {
        var @event = new UserCreatedEvent
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };

        await _publisher.PublishAsync(
            @event,
            "recovery_system_exchange",
            "user.created");

        _logger.LogInformation("Published UserCreatedEvent for user {UserId}", user.Id);
    }

    public async Task PublishUserUpdatedEventAsync(ApplicationUser user)
    {
        var @event = new UserUpdatedEvent
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            UpdatedAt = DateTime.UtcNow
        };

        await _publisher.PublishAsync(
            @event,
            "recovery_system_exchange",
            "user.updated");

        _logger.LogInformation("Published UserUpdatedEvent for user {UserId}", user.Id);
    }

    public async Task PublishUserLoggedInEventAsync(ApplicationUser user)
    {
        var context = _httpContextAccessor.HttpContext;
        var ipAddress = context?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var userAgent = context?.Request.Headers.UserAgent.ToString() ?? "unknown";

        var @event = new UserLoggedInEvent
        {
            UserId = user.Id,
            Email = user.Email,
            LoginTime = DateTime.UtcNow,
            IpAddress = ipAddress,
            UserAgent = userAgent
        };

        await _publisher.PublishAsync(
            @event,
            "recovery_system_exchange",
            "user.logged_in");

        _logger.LogInformation("Published UserLoggedInEvent for user {UserId}", user.Id);
    }
}