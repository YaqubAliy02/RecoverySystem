using RecoverySystem.BuildingBlocks.Messaging.Messages;
using System;

namespace RecoverySystem.BuildingBlocks.Events.Events
{
    public record VitalSignsRecordedEvent : IntegrationEvent
    {
        public string MonitoringId { get; init; }
        public string PatientId { get; init; }
        public int HeartRate { get; init; }
        public string BloodPressure { get; init; }
        public double Temperature { get; init; }
        public int RespiratoryRate { get; init; }
        public int OxygenSaturation { get; init; }
        public int PainLevel { get; init; }
        public DateTime Timestamp { get; init; }
    }

    public record AlertCreatedEvent : IntegrationEvent
    {
        public string AlertId { get; init; }
        public string PatientId { get; init; }
        public string PatientName { get; init; }
        public string Type { get; init; }
        public string Severity { get; init; }
        public string Message { get; init; }
        public DateTime CreatedAt { get; init; }
    }

    public record AlertResolvedEvent : IntegrationEvent
    {
        public string AlertId { get; init; }
        public string PatientId { get; init; }
        public string ResolvedById { get; init; }
        public string ResolvedByName { get; init; }
        public DateTime ResolvedAt { get; init; }
    }

    public record SystemHealthChangedEvent : IntegrationEvent
    {
        public string ServiceName { get; init; }
        public string Status { get; init; }
        public string Description { get; init; }
        public DateTime Timestamp { get; init; }
    }
}