using RecoverySystem.BuildingBlocks.Events;
using RecoverySystem.BuildingBlocks.Events.Events;
using RecoverySystem.BuildingBlocks.Messaging.RabbitMQ;
using RecoverySystem.RehabilitationService.DTOs;
using PatientUpdatedEvent = RecoverySystem.BuildingBlocks.Events.Events.PatientUpdatedEvent;

namespace RecoverySystem.RehabilitationService.Services
{
    public class EventConsumerService : BackgroundService
    {
        private readonly IRabbitMQConsumer _consumer;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EventConsumerService> _logger;

        public EventConsumerService(
            IRabbitMQConsumer consumer,
            IServiceProvider serviceProvider,
            ILogger<EventConsumerService> logger)
        {
            _consumer = consumer;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Rehabilitation Service Event Consumer is starting");

            try
            {
                // Subscribe to patient events
                _consumer.Subscribe<PatientCreatedEvent>(
                    "recovery_system_exchange",
                    "rehabilitation_service_patient_created_queue",
                    "patient.created",
                    HandlePatientCreatedEvent);

                _consumer.Subscribe<PatientUpdatedEvent>(
                    "recovery_system_exchange",
                    "rehabilitation_service_patient_updated_queue",
                    "patient.updated",
                    HandlePatientUpdatedEvent);

                // Subscribe to case events
                _consumer.Subscribe<CaseCreatedEvent>(
                    "recovery_system_exchange",
                    "rehabilitation_service_case_created_queue",
                    "case.created",
                    HandleCaseCreatedEvent);

                _consumer.Subscribe<CaseStatusChangedEvent>(
                    "recovery_system_exchange",
                    "rehabilitation_service_case_status_changed_queue",
                    "case.status.changed",
                    HandleCaseStatusChangedEvent);

                // Subscribe to recommendation events
                _consumer.Subscribe<RecommendationCreatedEvent>(
                    "recovery_system_exchange",
                    "rehabilitation_service_recommendation_created_queue",
                    "recommendation.created",
                    HandleRecommendationCreatedEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting up event subscriptions");
            }

            return Task.CompletedTask;
        }

        private Task HandlePatientCreatedEvent(PatientCreatedEvent @event)
        {
            _logger.LogInformation("Received PatientCreatedEvent for patient {PatientId}", @event.PatientId);
            // No specific action needed for patient creation in the rehabilitation service
            return Task.CompletedTask;
        }

        private Task HandlePatientUpdatedEvent(PatientUpdatedEvent @event)
        {
            _logger.LogInformation("Received PatientUpdatedEvent for patient {PatientId}", @event.PatientId);
            // No specific action needed for patient updates in the rehabilitation service
            return Task.CompletedTask;
        }

        private Task HandleCaseCreatedEvent(CaseCreatedEvent @event)
        {
            _logger.LogInformation("Received CaseCreatedEvent for case {CaseId}", @event.CaseId);
            // No specific action needed for case creation in the rehabilitation service
            return Task.CompletedTask;
        }

        private async Task HandleCaseStatusChangedEvent(CaseStatusChangedEvent @event)
        {
            _logger.LogInformation("Received CaseStatusChangedEvent for case {CaseId}", @event.CaseId);

            // If case is closed, complete any active rehabilitation programs for this case
            if (@event.NewStatus.ToLower() == "closed")
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var rehabilitationService = scope.ServiceProvider.GetRequiredService<IRehabilitationService>();

                    var programs = await rehabilitationService.GetRehabilitationProgramsForCaseAsync(Guid.Parse(@event.CaseId));
                    foreach (var program in programs)
                    {
                        if (program.Status == Models.RehabilitationStatus.InProgress ||
                            program.Status == Models.RehabilitationStatus.Planned)
                        {
                            await rehabilitationService.UpdateRehabilitationProgramStatusAsync(
                                program.Id,
                                Models.RehabilitationStatus.Completed);

                            _logger.LogInformation("Completed rehabilitation program {ProgramId} due to case closure", program.Id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing CaseStatusChangedEvent for case {CaseId}", @event.CaseId);
                    throw;
                }
            }
        }

        private async Task HandleRecommendationCreatedEvent(RecommendationCreatedEvent @event)
        {
            // Defensive: Validate all GUIDs before parsing (except CaseId can be optional)
            if (string.IsNullOrWhiteSpace(@event.PatientId) || !Guid.TryParse(@event.PatientId, out var patientGuid))
            {
                _logger.LogError("Invalid or missing PatientId in RecommendationCreatedEvent: '{PatientId}'", @event.PatientId);
                return;
            }
            if (string.IsNullOrWhiteSpace(@event.CreatedById) || !Guid.TryParse(@event.CreatedById, out var assignedToGuid))
            {
                _logger.LogError("Invalid or missing CreatedById in RecommendationCreatedEvent: '{CreatedById}'", @event.CreatedById);
                return;
            }

            // CaseId is optional
            Guid? caseGuid = null;
            if (!string.IsNullOrWhiteSpace(@event.CaseId) && Guid.TryParse(@event.CaseId, out var tmpCaseGuid))
            {
                caseGuid = tmpCaseGuid;
            } // else, caseGuid stays null

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var rehabilitationService = scope.ServiceProvider.GetRequiredService<IRehabilitationService>();

                var createDto = new CreateRehabilitationProgramDto
                {
                    Title = $"Rehabilitation Program: {@event.Title}",
                    Description = @event.Description,
                    PatientId = patientGuid,
                    PatientName = @event.PatientName,
                    CaseId = caseGuid,
                    AssignedToId = assignedToGuid,
                    AssignedToName = @event.CreatedByName,
                    StartDate = DateTime.UtcNow.AddDays(1),
                    EndDate = DateTime.UtcNow.AddDays(30),
                    Notes = $"Created automatically based on recommendation: {@event.RecommendationId}"
                };

                var program = await rehabilitationService.CreateRehabilitationProgramAsync(createDto);

                _logger.LogInformation("Created rehabilitation program {ProgramId} based on recommendation {RecommendationId}",
                    program.Id, @event.RecommendationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing RecommendationCreatedEvent for recommendation {RecommendationId}", @event.RecommendationId);
            }

        }
    }
}