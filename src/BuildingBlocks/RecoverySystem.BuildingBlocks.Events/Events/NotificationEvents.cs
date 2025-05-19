using RecoverySystem.BuildingBlocks.Messaging.Messages;

namespace RecoverySystem.BuildingBlocks.Events.Events;

public record NotificationCreatedEvent : IntegrationEvent
{
    public string NotificationId { get; init; }
    public string Title { get; init; }
    public string Message { get; init; }
    public string Type { get; init; }
    public string TargetUserId { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record NotificationReadEvent : IntegrationEvent
{
    public string NotificationId { get; init; }
    public string UserId { get; init; }
    public DateTime ReadAt { get; init; }
}