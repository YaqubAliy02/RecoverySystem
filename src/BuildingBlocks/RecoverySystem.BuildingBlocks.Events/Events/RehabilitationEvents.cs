using RecoverySystem.BuildingBlocks.Messaging.Messages;
using System;

namespace RecoverySystem.BuildingBlocks.Events.Events
{
    public record RehabilitationStartedEvent : IntegrationEvent
    {
        public string RehabilitationId { get; init; }
        public string Title { get; init; }
        public string PatientId { get; init; }
        public string PatientName { get; init; }
        public string CaseId { get; init; }
        public string AssignedToId { get; init; }
        public string AssignedToName { get; init; }
        public DateTime StartDate { get; init; }
    }

    public record RehabilitationStatusChangedEvent : IntegrationEvent
    {
        public string RehabilitationId { get; init; }
        public string PatientId { get; init; }
        public string OldStatus { get; init; }
        public string NewStatus { get; init; }
        public DateTime UpdatedAt { get; init; }
    }

    public record RehabilitationSessionCompletedEvent : IntegrationEvent
    {
        public string SessionId { get; init; }
        public string RehabilitationId { get; init; }
        public string PatientId { get; init; }
        public DateTime CompletedDate { get; init; }
        public int PainLevel { get; init; }
        public int FatigueLevel { get; init; }
        public int SatisfactionLevel { get; init; }
    }

    public record ProgressReportCreatedEvent : IntegrationEvent
    {
        public string ReportId { get; init; }
        public string RehabilitationId { get; init; }
        public string PatientId { get; init; }
        public string PatientName { get; init; }
        public double ComplianceRate { get; init; }
        public DateTime ReportDate { get; init; }
    }
}