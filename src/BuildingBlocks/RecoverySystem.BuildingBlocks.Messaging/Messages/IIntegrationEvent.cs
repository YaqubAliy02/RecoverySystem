namespace RecoverySystem.BuildingBlocks.Messaging.Messages;

public interface IIntegrationEvent
{
    Guid Id { get; }
    DateTime CreationDate { get; }
}
