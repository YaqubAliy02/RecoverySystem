using RecoverySystem.BuildingBlocks.Events.Events;
using RecoverySystem.BuildingBlocks.Messaging.RabbitMQ;
using RecoverySystem.CaseService.Models;

namespace RecoverySystem.CaseService.Services
{
    public class EventPublisher
    {
        private readonly IRabbitMQPublisher _publisher;
        private readonly ILogger<EventPublisher> _logger;

        public EventPublisher(IRabbitMQPublisher publisher, ILogger<EventPublisher> logger)
        {
            _publisher = publisher;
            _logger = logger;
        }

        public async Task PublishCaseCreatedEventAsync(Case @case)
        {
            var @event = new CaseCreatedEvent
            {
                CaseId = @case.Id.ToString(),
                Title = @case.Title,
                PatientId = @case.PatientId.ToString(),
                Status = @case.Status.ToString(),
                AssignedToId = @case.AssignedToId.ToString(),
                CreatedById = @case.CreatedById.ToString(),
                CreatedAt = @case.CreatedAt
            };

            await _publisher.PublishAsync(
                @event,
                "recovery_system_exchange",
                "case.created");

            _logger.LogInformation("Published CaseCreatedEvent for case {CaseId}", @case.Id);
        }

        public async Task PublishCaseUpdatedEventAsync(Case @case)
        {
            var @event = new CaseUpdatedEvent
            {
                CaseId = @case.Id.ToString(),
                Title = @case.Title,
                Status = @case.Status.ToString(),
                AssignedToId = @case.AssignedToId.ToString(),
                UpdatedAt = @case.UpdatedAt ?? DateTime.UtcNow
            };

            await _publisher.PublishAsync(
                @event,
                "recovery_system_exchange",
                "case.updated");

            _logger.LogInformation("Published CaseUpdatedEvent for case {CaseId}", @case.Id);
        }

        public async Task PublishCaseStatusChangedEventAsync(Case @case, CaseStatus oldStatus)
        {
            var @event = new CaseStatusChangedEvent
            {
                CaseId = @case.Id.ToString(),
                OldStatus = oldStatus.ToString(),
                NewStatus = @case.Status.ToString(),
                UpdatedAt = @case.UpdatedAt ?? DateTime.UtcNow
            };

            await _publisher.PublishAsync(
                @event,
                "recovery_system_exchange",
                "case.status.changed");

            _logger.LogInformation("Published CaseStatusChangedEvent for case {CaseId}", @case.Id);
        }

        public async Task PublishCaseNoteAddedEventAsync(CaseNote note, string createdByName)
        {
            var @event = new CaseNoteAddedEvent
            {
                NoteId = note.Id.ToString(),
                CaseId = note.CaseId.ToString(),
                Content = note.Content,
                CreatedById = note.CreatedById.ToString(),
                CreatedByName = createdByName,
                CreatedAt = note.CreatedAt
            };

            await _publisher.PublishAsync(
                @event,
                "recovery_system_exchange",
                "case.note.added");

            _logger.LogInformation("Published CaseNoteAddedEvent for note {NoteId}", note.Id);
        }
    }
}