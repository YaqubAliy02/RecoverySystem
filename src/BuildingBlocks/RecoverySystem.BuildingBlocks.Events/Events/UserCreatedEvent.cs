using RecoverySystem.BuildingBlocks.Messaging.Messages;

namespace RecoverySystem.BuildingBlocks.Events;

public record UserCreatedEvent : IntegrationEvent
{
    public string UserId { get; init; }
    public string FullName { get; init; }
    public string Email { get; init; }
    public string Role { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record UserUpdatedEvent : IntegrationEvent
{
    public string UserId { get; init; }
    public string FullName { get; init; }
    public string Email { get; init; }
    public string Role { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record UserLoggedInEvent : IntegrationEvent
{
    public string UserId { get; init; }
    public string Email { get; init; }
    public DateTime LoginTime { get; init; }
    public string IpAddress { get; init; }
    public string UserAgent { get; init; }
}