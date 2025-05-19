using RecoverySystem.BuildingBlocks.Messaging.Messages;

namespace RecoverySystem.BuildingBlocks.Events.Events;

public record RecommendationCreatedEvent : IntegrationEvent
{
    public string RecommendationId { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public string Type { get; init; }
    public string PatientId { get; init; }
    public string PatientName { get; init; }
    public string CaseId { get; init; }
    public string Priority { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public List<string> Tags { get; init; }
    public string CreatedById { get; init; }
    public string CreatedByName { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record RecommendationApprovedEvent : IntegrationEvent
{
    public string RecommendationId { get; init; }
    public string PatientId { get; init; }
    public string ApprovedById { get; init; }
    public string ApprovedByName { get; init; }
    public DateTime ApprovedAt { get; init; }
}

public record RecommendationStatusChangedEvent : IntegrationEvent
{
    public string RecommendationId { get; init; }
    public string PatientId { get; init; }
    public string OldStatus { get; init; }
    public string NewStatus { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record RecommendationFeedbackAddedEvent : IntegrationEvent
{
    public string FeedbackId { get; init; }
    public string RecommendationId { get; init; }
    public string PatientId { get; init; }
    public int Rating { get; init; }
    public bool IsEffective { get; init; }
    public bool IsPatientFeedback { get; init; }
    public DateTime CreatedAt { get; init; }
}