using RecoverySystem.BuildingBlocks.Events.Events;
using RecoverySystem.BuildingBlocks.Messaging.RabbitMQ;
using RecoverySystem.RehabilitationService.Models;

namespace RecoverySystem.RehabilitationService.Services
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

        public async Task PublishRehabilitationStartedEventAsync(RehabilitationProgram program)
        {
            try
            {
                _logger.LogInformation("Publishing RehabilitationStartedEvent for program {Id}", program.Id);

                var @event = new RehabilitationStartedEvent
                {
                    RehabilitationId = program.Id.ToString(),
                    Title = program.Title,
                    PatientId = program.PatientId.ToString(),
                    PatientName = program.PatientName,
                    CaseId = program.CaseId.ToString(),
                    AssignedToId = program.AssignedToId.ToString(),
                    AssignedToName = program.AssignedToName,
                    StartDate = program.StartDate
                };

                await _publisher.PublishAsync(
                    @event,
                    "recovery_system_exchange",
                    "rehabilitation.started");

                _logger.LogInformation("Successfully published RehabilitationStartedEvent for program {Id}", program.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish RehabilitationStartedEvent for program {Id}", program.Id);
            }
        }

        public async Task PublishRehabilitationStatusChangedEventAsync(RehabilitationProgram program, RehabilitationStatus oldStatus)
        {
            try
            {
                _logger.LogInformation("Publishing RehabilitationStatusChangedEvent for program {Id}", program.Id);

                var @event = new RehabilitationStatusChangedEvent
                {
                    RehabilitationId = program.Id.ToString(),
                    PatientId = program.PatientId.ToString(),
                    OldStatus = oldStatus.ToString(),
                    NewStatus = program.Status.ToString(),
                    UpdatedAt = program.UpdatedAt ?? DateTime.UtcNow
                };

                await _publisher.PublishAsync(
                    @event,
                    "recovery_system_exchange",
                    "rehabilitation.status.changed");

                _logger.LogInformation("Successfully published RehabilitationStatusChangedEvent for program {Id}", program.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish RehabilitationStatusChangedEvent for program {Id}", program.Id);
            }
        }

        public async Task PublishRehabilitationSessionCompletedEventAsync(RehabilitationSession session)
        {
            try
            {
                _logger.LogInformation("Publishing RehabilitationSessionCompletedEvent for session {Id}", session.Id);

                var @event = new RehabilitationSessionCompletedEvent
                {
                    SessionId = session.Id.ToString(),
                    RehabilitationId = session.RehabilitationProgramId.ToString(),
                    PatientId = "unknown", // In a real system, you'd get the patient ID from the program
                    CompletedDate = session.CompletedDate ?? DateTime.UtcNow,
                    PainLevel = session.PainLevel,
                    FatigueLevel = session.FatigueLevel,
                    SatisfactionLevel = session.SatisfactionLevel
                };

                await _publisher.PublishAsync(
                    @event,
                    "recovery_system_exchange",
                    "rehabilitation.session.completed");

                _logger.LogInformation("Successfully published RehabilitationSessionCompletedEvent for session {Id}", session.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish RehabilitationSessionCompletedEvent for session {Id}", session.Id);
            }
        }

        public async Task PublishProgressReportCreatedEventAsync(ProgressReport report)
        {
            try
            {
                _logger.LogInformation("Publishing ProgressReportCreatedEvent for report {Id}", report.Id);

                var @event = new ProgressReportCreatedEvent
                {
                    ReportId = report.Id.ToString(),
                    RehabilitationId = report.RehabilitationProgramId.ToString(),
                    PatientId = report.PatientId.ToString(),
                    PatientName = report.PatientName,
                    ComplianceRate = report.ComplianceRate,
                    ReportDate = report.ReportDate
                };

                await _publisher.PublishAsync(
                    @event,
                    "recovery_system_exchange",
                    "rehabilitation.progress.report.created");

                _logger.LogInformation("Successfully published ProgressReportCreatedEvent for report {Id}", report.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish ProgressReportCreatedEvent for report {Id}", report.Id);
            }
        }
    }
}