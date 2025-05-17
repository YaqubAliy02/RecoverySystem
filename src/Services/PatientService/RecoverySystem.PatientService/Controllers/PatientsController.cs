// Controllers/PatientsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecoverySystem.PatientService.Data;
using RecoverySystem.PatientService.Models;

namespace RecoverySystem.PatientService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PatientsController : ControllerBase
{
    private readonly PatientDbContext _context;
    private readonly ILogger<PatientsController> _logger;

    public PatientsController(PatientDbContext context, ILogger<PatientsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Patient>>> GetPatients()
    {
        return await _context.Patients
            .Include(p => p.Vitals.OrderByDescending(v => v.Date).Take(1))
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Patient>> GetPatient(string id)
    {
        var patient = await _context.Patients
            .Include(p => p.Vitals.OrderByDescending(v => v.Date))
            .Include(p => p.Notes.OrderByDescending(n => n.Date))
            .FirstOrDefaultAsync(p => p.Id == id);

        if (patient == null)
        {
            return NotFound();
        }

        return patient;
    }

    [HttpPost]
    public async Task<ActionResult<Patient>> CreatePatient(Patient patient)
    {
        patient.Id = Guid.NewGuid().ToString();
        patient.CreatedAt = DateTime.UtcNow;
        patient.UpdatedAt = DateTime.UtcNow;

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePatient(string id, Patient patient)
    {
        if (id != patient.Id)
        {
            return BadRequest();
        }

        patient.UpdatedAt = DateTime.UtcNow;
        _context.Entry(patient).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
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
    public async Task<ActionResult<PatientVital>> AddVital(string id, PatientVital vital)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null)
        {
            return NotFound();
        }

        vital.Id = Guid.NewGuid().ToString();
        vital.PatientId = id;
        vital.Date = DateTime.UtcNow;

        _context.PatientVitals.Add(vital);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, vital);
    }

    [HttpPost("{id}/notes")]
    public async Task<ActionResult<PatientNote>> AddNote(string id, PatientNote note)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null)
        {
            return NotFound();
        }

        note.Id = Guid.NewGuid().ToString();
        note.PatientId = id;
        note.Date = DateTime.UtcNow;

        _context.PatientNotes.Add(note);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, note);
    }

    private bool PatientExists(string id)
    {
        return _context.Patients.Any(e => e.Id == id);
    }
}