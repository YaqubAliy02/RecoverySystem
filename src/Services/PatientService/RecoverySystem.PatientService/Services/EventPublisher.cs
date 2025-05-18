using RecoverySystem.BuildingBlocks.Events;
using RecoverySystem.BuildingBlocks.Messaging.RabbitMQ;
using RecoverySystem.PatientService.Models;

namespace RecoverySystem.PatientService.Services;

public class EventPublisher
{
    private readonly IRabbitMQPublisher _publisher;
    private readonly ILogger<EventPublisher> _logger;

    public EventPublisher(IRabbitMQPublisher publisher, ILogger<EventPublisher> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public async Task PublishPatientCreatedEventAsync(Patient patient)
    {
        var @event = new PatientCreatedEvent
        {
            PatientId = patient.Id,
            Name = patient.Name,
            Age = patient.Age,
            Gender = patient.Gender,
            Email = patient.Email,
            Phone = patient.Phone,
            Status = patient.Status,
            CreatedAt = patient.CreatedAt
        };

        await _publisher.PublishAsync(
            @event,
            "recovery_system_exchange",
            "patient.created");

        _logger.LogInformation("Published PatientCreatedEvent for patient {PatientId}", patient.Id);
    }

    public async Task PublishPatientUpdatedEventAsync(Patient patient)
    {
        var @event = new PatientUpdatedEvent
        {
            PatientId = patient.Id,
            Name = patient.Name,
            Status = patient.Status,
            UpdatedAt = patient.UpdatedAt
        };

        await _publisher.PublishAsync(
            @event,
            "recovery_system_exchange",
            "patient.updated");

        _logger.LogInformation("Published PatientUpdatedEvent for patient {PatientId}", patient.Id);
    }

    public async Task PublishPatientVitalRecordedEventAsync(PatientVital vital)
    {
        var @event = new PatientVitalRecordedEvent
        {
            PatientId = vital.PatientId,
            VitalId = vital.Id,
            HeartRate = vital.HeartRate,
            BloodPressure = vital.BloodPressure,
            Temperature = vital.Temperature,
            OxygenSaturation = vital.OxygenSaturation,
            RecordedAt = vital.Date
        };

        await _publisher.PublishAsync(
            @event,
            "recovery_system_exchange",
            "patient.vital_recorded");

        _logger.LogInformation("Published PatientVitalRecordedEvent for patient {PatientId}", vital.PatientId);
    }

    public async Task PublishPatientNoteAddedEventAsync(PatientNote note)
    {
        var @event = new PatientNoteAddedEvent
        {
            PatientId = note.PatientId,
            NoteId = note.Id,
            Content = note.Content,
            AuthorId = note.AuthorId,
            AuthorName = note.AuthorName,
            CreatedAt = note.Date
        };

        await _publisher.PublishAsync(
            @event,
            "recovery_system_exchange",
            "patient.note_added");

        _logger.LogInformation("Published PatientNoteAddedEvent for patient {PatientId}", note.PatientId);
    }

    public async Task PublishPatientRecommendationAddedEventAsync(PatientRecommendation recommendation)
    {
        var @event = new PatientRecommendationAddedEvent
        {
            PatientId = recommendation.PatientId,
            RecommendationId = recommendation.Id,
            Title = recommendation.Title,
            Type = recommendation.Type,
            CreatedById = recommendation.CreatedById,
            CreatedByName = recommendation.CreatedByName,
            CreatedAt = recommendation.CreatedAt
        };

        await _publisher.PublishAsync(
            @event,
            "recovery_system_exchange",
            "patient.recommendation_added");

        _logger.LogInformation("Published PatientRecommendationAddedEvent for patient {PatientId}", recommendation.PatientId);
    }

    public async Task PublishPatientRehabilitationStartedEventAsync(PatientRehabilitation rehabilitation)
    {
        var @event = new PatientRehabilitationStartedEvent
        {
            PatientId = rehabilitation.PatientId,
            RehabilitationId = rehabilitation.Id,
            Title = rehabilitation.Title,
            AssignedToId = rehabilitation.AssignedToId,
            AssignedToName = rehabilitation.AssignedToName,
            StartDate = rehabilitation.StartDate
        };

        await _publisher.PublishAsync(
            @event,
            "recovery_system_exchange",
            "patient.rehabilitation_started");

        _logger.LogInformation("Published PatientRehabilitationStartedEvent for patient {PatientId}", rehabilitation.PatientId);
    }
}