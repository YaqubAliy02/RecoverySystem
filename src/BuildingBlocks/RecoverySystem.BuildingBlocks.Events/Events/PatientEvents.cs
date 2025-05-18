using RecoverySystem.BuildingBlocks.Messaging.Messages;

namespace RecoverySystem.BuildingBlocks.Events;

public record PatientCreatedEvent : IntegrationEvent
{
    public string PatientId { get; init; }
    public string Name { get; init; }
    public int Age { get; init; }
    public string Gender { get; init; }
    public string Email { get; init; }
    public string Phone { get; init; }
    public string Status { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record PatientUpdatedEvent : IntegrationEvent
{
    public string PatientId { get; init; }
    public string Name { get; init; }
    public string Status { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record PatientVitalRecordedEvent : IntegrationEvent
{
    public string PatientId { get; init; }
    public string VitalId { get; init; }
    public int HeartRate { get; init; }
    public string BloodPressure { get; init; }
    public double Temperature { get; init; }
    public int OxygenSaturation { get; init; }
    public DateTime RecordedAt { get; init; }
}

public record PatientNoteAddedEvent : IntegrationEvent
{
    public string PatientId { get; init; }
    public string NoteId { get; init; }
    public string Content { get; init; }
    public string AuthorId { get; init; }
    public string AuthorName { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record PatientRecommendationAddedEvent : IntegrationEvent
{
    public string PatientId { get; init; }
    public string RecommendationId { get; init; }
    public string Title { get; init; }
    public string Type { get; init; }
    public string CreatedById { get; init; }
    public string CreatedByName { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record PatientRehabilitationStartedEvent : IntegrationEvent
{
    public string PatientId { get; init; }
    public string RehabilitationId { get; init; }
    public string Title { get; init; }
    public string AssignedToId { get; init; }
    public string AssignedToName { get; init; }
    public DateTime StartDate { get; init; }
}