using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecoverySystem.PatientService.Data;
using RecoverySystem.PatientService.DTOs;
using RecoverySystem.PatientService.DTOs.PatientRehabilitations;
using RecoverySystem.PatientService.Models;
using RecoverySystem.PatientService.Services;

namespace RecoverySystem.PatientService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PatientsController : ControllerBase
{
    private readonly PatientDbContext _context;
    private readonly EventPublisher _eventPublisher;
    private readonly IMapper _mapper;
    private readonly ILogger<PatientsController> _logger;

    public PatientsController(
        PatientDbContext context,
        EventPublisher eventPublisher,
        IMapper mapper,
        ILogger<PatientsController> logger)
    {
        _context = context;
        _eventPublisher = eventPublisher;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PatientDto>>> GetPatients()
    {
        var patients = await _context.Patients
            .Include(p => p.Vitals.OrderByDescending(v => v.Date).Take(1))
            .ToListAsync();

        return _mapper.Map<List<PatientDto>>(patients);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PatientDetailDto>> GetPatient(string id)
    {
        var patient = await _context.Patients
            .Include(p => p.Vitals.OrderByDescending(v => v.Date))
            .Include(p => p.Notes.OrderByDescending(n => n.Date))
            .FirstOrDefaultAsync(p => p.Id == id);

        if (patient == null)
        {
            return NotFound();
        }

        var patientDto = _mapper.Map<PatientDetailDto>(patient);

        // Get recommendations
        var recommendations = await _context.PatientRecommendations
            .Where(r => r.PatientId == id)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        patientDto.Recommendations = _mapper.Map<List<PatientRecommendationDto>>(recommendations);

        // Get rehabilitations
        var rehabilitations = await _context.PatientRehabilitations
            .Include(r => r.Exercises)
            .Where(r => r.PatientId == id)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        patientDto.Rehabilitations = _mapper.Map<List<PatientRehabilitationDto>>(rehabilitations);

        return patientDto;
    }

    [HttpPost]
    public async Task<ActionResult<PatientDto>> CreatePatient(PatientCreateDto patientDto)
    {
        var patient = _mapper.Map<Patient>(patientDto);

        patient.Id = Guid.NewGuid().ToString();
        patient.CreatedAt = DateTime.UtcNow;
        patient.UpdatedAt = DateTime.UtcNow;

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        // Publish event
        await _eventPublisher.PublishPatientCreatedEventAsync(patient);

        var resultDto = _mapper.Map<PatientDto>(patient);
        return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, resultDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePatient(string id, PatientUpdateDto patientDto)
    {
        var existingPatient = await _context.Patients.FindAsync(id);
        if (existingPatient == null)
        {
            return NotFound();
        }

        // Update properties using AutoMapper
        _mapper.Map(patientDto, existingPatient);
        existingPatient.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();

            // Publish event
            await _eventPublisher.PublishPatientUpdatedEventAsync(existingPatient);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PatientExists(id))
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
    public async Task<IActionResult> DeletePatient(string id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null)
        {
            return NotFound();
        }

        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id}/vitals")]
    public async Task<ActionResult<PatientVitalDto>> AddVital(string id, PatientVitalCreateDto vitalDto)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null)
        {
            return NotFound();
        }

        var vital = _mapper.Map<PatientVital>(vitalDto);
        vital.Id = Guid.NewGuid().ToString();
        vital.PatientId = id;
        vital.Date = DateTime.UtcNow;

        _context.PatientVitals.Add(vital);
        await _context.SaveChangesAsync();

        // Publish event
        await _eventPublisher.PublishPatientVitalRecordedEventAsync(vital);

        var resultDto = _mapper.Map<PatientVitalDto>(vital);
        return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, resultDto);
    }

    [HttpPost("{id}/notes")]
    public async Task<ActionResult<PatientNoteDto>> AddNote(string id, PatientNoteCreateDto noteDto)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null)
        {
            return NotFound();
        }

        var note = _mapper.Map<PatientNote>(noteDto);
        note.Id = Guid.NewGuid().ToString();
        note.PatientId = id;
        note.Date = DateTime.UtcNow;

        _context.PatientNotes.Add(note);
        await _context.SaveChangesAsync();

        // Publish event
        await _eventPublisher.PublishPatientNoteAddedEventAsync(note);

        var resultDto = _mapper.Map<PatientNoteDto>(note);
        return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, resultDto);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<PatientDto>>> SearchPatients([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return await GetPatients();
        }

        var patients = await _context.Patients
            .Where(p => p.Name.Contains(query) ||
                         p.Email.Contains(query) ||
                         p.Phone.Contains(query))
            .Include(p => p.Vitals.OrderByDescending(v => v.Date).Take(1))
            .ToListAsync();

        return _mapper.Map<List<PatientDto>>(patients);
    }

    [HttpGet("by-status/{status}")]
    public async Task<ActionResult<IEnumerable<PatientDto>>> GetPatientsByStatus(string status)
    {
        var patients = await _context.Patients
            .Where(p => p.Status == status)
            .Include(p => p.Vitals.OrderByDescending(v => v.Date).Take(1))
            .ToListAsync();

        return _mapper.Map<List<PatientDto>>(patients);
    }

    private bool PatientExists(string id)
    {
        return _context.Patients.Any(e => e.Id == id);
    }
}