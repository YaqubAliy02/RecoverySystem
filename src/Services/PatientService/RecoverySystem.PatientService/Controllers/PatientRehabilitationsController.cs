using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecoverySystem.PatientService.Data;
using RecoverySystem.PatientService.DTOs.PatientRehabilitations;
using RecoverySystem.PatientService.Models;
using RecoverySystem.PatientService.Services;

namespace RecoverySystem.PatientService.Controllers;

[ApiController]
[Route("api/patients/{patientId}/rehabilitations")]
[Authorize]
public class PatientRehabilitationsController : ControllerBase
{
    private readonly PatientDbContext _context;
    private readonly EventPublisher _eventPublisher;
    private readonly IMapper _mapper;
    private readonly ILogger<PatientRehabilitationsController> _logger;

    public PatientRehabilitationsController(
        PatientDbContext context,
        EventPublisher eventPublisher,
        IMapper mapper,
        ILogger<PatientRehabilitationsController> logger)
    {
        _context = context;
        _eventPublisher = eventPublisher;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PatientRehabilitationDto>>> GetRehabilitations(string patientId)
    {
        var patient = await _context.Patients.FindAsync(patientId);
        if (patient == null)
        {
            return NotFound();
        }

        var rehabilitations = await _context.PatientRehabilitations
            .Where(r => r.PatientId == patientId)
            .Include(r => r.Exercises)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return _mapper.Map<List<PatientRehabilitationDto>>(rehabilitations);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PatientRehabilitationDto>> GetRehabilitation(string patientId, string id)
    {
        var rehabilitation = await _context.PatientRehabilitations
            .Include(r => r.Exercises)
            .FirstOrDefaultAsync(r => r.PatientId == patientId && r.Id == id);

        if (rehabilitation == null)
        {
            return NotFound();
        }

        return _mapper.Map<PatientRehabilitationDto>(rehabilitation);
    }

    [HttpPost]
    public async Task<ActionResult<PatientRehabilitationDto>> CreateRehabilitation(
        string patientId,
        PatientRehabilitationCreateDto rehabilitationDto)
    {
        var patient = await _context.Patients.FindAsync(patientId);
        if (patient == null)
        {
            return NotFound();
        }

        var rehabilitation = _mapper.Map<PatientRehabilitation>(rehabilitationDto);
        rehabilitation.Id = Guid.NewGuid().ToString();
        rehabilitation.PatientId = patientId;
        rehabilitation.CreatedAt = DateTime.UtcNow;
        rehabilitation.UpdatedAt = DateTime.UtcNow;

        // Add exercises if provided
        if (rehabilitationDto.Exercises != null && rehabilitationDto.Exercises.Any())
        {
            foreach (var exerciseDto in rehabilitationDto.Exercises)
            {
                var exercise = _mapper.Map<RehabilitationExercise>(exerciseDto);
                exercise.Id = Guid.NewGuid().ToString();
                exercise.RehabilitationId = rehabilitation.Id;
                exercise.CreatedAt = DateTime.UtcNow;
                exercise.UpdatedAt = DateTime.UtcNow;

                rehabilitation.Exercises.Add(exercise);
            }
        }

        _context.PatientRehabilitations.Add(rehabilitation);
        await _context.SaveChangesAsync();

        // Publish event
        await _eventPublisher.PublishPatientRehabilitationStartedEventAsync(rehabilitation);

        var resultDto = _mapper.Map<PatientRehabilitationDto>(rehabilitation);
        return CreatedAtAction(
            nameof(GetRehabilitation),
            new { patientId = patientId, id = rehabilitation.Id },
            resultDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRehabilitation(
        string patientId,
        string id,
        PatientRehabilitationUpdateDto rehabilitationDto)
    {
        var existingRehabilitation = await _context.PatientRehabilitations
            .FirstOrDefaultAsync(r => r.PatientId == patientId && r.Id == id);

        if (existingRehabilitation == null)
        {
            return NotFound();
        }

        // Update properties using AutoMapper
        _mapper.Map(rehabilitationDto, existingRehabilitation);
        existingRehabilitation.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!RehabilitationExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRehabilitation(string patientId, string id)
    {
        var rehabilitation = await _context.PatientRehabilitations
            .FirstOrDefaultAsync(r => r.PatientId == patientId && r.Id == id);

        if (rehabilitation == null)
        {
            return NotFound();
        }

        _context.PatientRehabilitations.Remove(rehabilitation);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{rehabilitationId}/exercises")]
    public async Task<ActionResult<RehabilitationExerciseDto>> AddExercise(
        string patientId,
        string rehabilitationId,
        RehabilitationExerciseCreateDto exerciseDto)
    {
        var rehabilitation = await _context.PatientRehabilitations
            .FirstOrDefaultAsync(r => r.PatientId == patientId && r.Id == rehabilitationId);

        if (rehabilitation == null)
        {
            return NotFound();
        }

        var exercise = _mapper.Map<RehabilitationExercise>(exerciseDto);
        exercise.Id = Guid.NewGuid().ToString();
        exercise.RehabilitationId = rehabilitationId;
        exercise.CreatedAt = DateTime.UtcNow;
        exercise.UpdatedAt = DateTime.UtcNow;

        _context.RehabilitationExercises.Add(exercise);
        await _context.SaveChangesAsync();

        var resultDto = _mapper.Map<RehabilitationExerciseDto>(exercise);
        return CreatedAtAction(
            nameof(GetExercise),
            new { patientId = patientId, rehabilitationId = rehabilitationId, id = exercise.Id },
            resultDto);
    }

    [HttpGet("{rehabilitationId}/exercises/{id}")]
    public async Task<ActionResult<RehabilitationExerciseDto>> GetExercise(
        string patientId,
        string rehabilitationId,
        string id)
    {
        var exercise = await _context.RehabilitationExercises
            .FirstOrDefaultAsync(e => e.RehabilitationId == rehabilitationId && e.Id == id);

        if (exercise == null)
        {
            return NotFound();
        }

        return _mapper.Map<RehabilitationExerciseDto>(exercise);
    }

    [HttpPut("{rehabilitationId}/exercises/{id}")]
    public async Task<IActionResult> UpdateExercise(
        string patientId,
        string rehabilitationId,
        string id,
        RehabilitationExerciseUpdateDto exerciseDto)
    {
        var existingExercise = await _context.RehabilitationExercises
            .FirstOrDefaultAsync(e => e.RehabilitationId == rehabilitationId && e.Id == id);

        if (existingExercise == null)
        {
            return NotFound();
        }

        // Update properties using AutoMapper
        _mapper.Map(exerciseDto, existingExercise);
        existingExercise.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ExerciseExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpDelete("{rehabilitationId}/exercises/{id}")]
    public async Task<IActionResult> DeleteExercise(
        string patientId,
        string rehabilitationId,
        string id)
    {
        var exercise = await _context.RehabilitationExercises
            .FirstOrDefaultAsync(e => e.RehabilitationId == rehabilitationId && e.Id == id);

        if (exercise == null)
        {
            return NotFound();
        }

        _context.RehabilitationExercises.Remove(exercise);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool RehabilitationExists(string id)
    {
        return _context.PatientRehabilitations.Any(e => e.Id == id);
    }

    private bool ExerciseExists(string id)
    {
        return _context.RehabilitationExercises.Any(e => e.Id == id);
    }
}