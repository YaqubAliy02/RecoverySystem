using System;
using RecoverySystem.BuildingBlocks.Messaging.Messages;

namespace RecoverySystem.BuildingBlocks.Events.Events
{
    // This is a placeholder for the PatientUpdatedEvent
    // You would need to replace this with your actual implementation
    public record PatientUpdatedEvent : IntegrationEvent
    {
        public Guid PatientId { get; init; }
        public string Name { get; init; }
        public string Email { get; init; }
        public string PhoneNumber { get; init; }
        public DateTime UpdatedAt { get; init; }
    }
}