using RecoverySystem.BuildingBlocks.Messaging.Messages;

namespace RecoverySystem.BuildingBlocks.Events.Events
{
    public record CaseCreatedEvent : IntegrationEvent
    {
        public string CaseId { get; init; }
        public string Title { get; init; }
        public string PatientId { get; init; }
        public string Status { get; init; }
        public string AssignedToId { get; init; }
        public string CreatedById { get; init; }
        public DateTime CreatedAt { get; init; }
    }

    public record CaseUpdatedEvent : IntegrationEvent
    {
        public string CaseId { get; init; }
        public string Title { get; init; }
        public string Status { get; init; }
        public string AssignedToId { get; init; }
        public DateTime UpdatedAt { get; init; }
    }

    public record CaseStatusChangedEvent : IntegrationEvent
    {
        public string CaseId { get; init; }
        public string OldStatus { get; init; }
        public string NewStatus { get; init; }
        public DateTime UpdatedAt { get; init; }
    }

    public record CaseNoteAddedEvent : IntegrationEvent
    {
        public string NoteId { get; init; }
        public string CaseId { get; init; }
        public string Content { get; init; }
        public string CreatedById { get; init; }
        public string CreatedByName { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}