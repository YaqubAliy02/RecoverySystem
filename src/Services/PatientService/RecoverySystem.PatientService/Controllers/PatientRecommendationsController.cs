using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecoverySystem.PatientService.Data;
using RecoverySystem.PatientService.DTOs;
using RecoverySystem.PatientService.Models;
using RecoverySystem.PatientService.Services;

namespace RecoverySystem.PatientService.Controllers;

[ApiController]
[Route("api/patients/{patientId}/recommendations")]
[Authorize]
public class PatientRecommendationsController : ControllerBase
{
    private readonly PatientDbContext _context;
    private readonly EventPublisher _eventPublisher;
    private readonly IMapper _mapper;
    private readonly ILogger<PatientRecommendationsController> _logger;

    public PatientRecommendationsController(
        PatientDbContext context,
        EventPublisher eventPublisher,
        IMapper mapper,
        ILogger<PatientRecommendationsController> logger)
    {
        _context = context;
        _eventPublisher = eventPublisher;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PatientRecommendationDto>>> GetRecommendations(string patientId)
    {
        var patient = await _context.Patients.FindAsync(patientId);
        if (patient == null)
        {
            return NotFound();
        }

        var recommendations = await _context.PatientRecommendations
            .Where(r => r.PatientId == patientId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return _mapper.Map<List<PatientRecommendationDto>>(recommendations);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PatientRecommendationDto>> GetRecommendation(string patientId, string id)
    {
        var recommendation = await _context.PatientRecommendations
            .FirstOrDefaultAsync(r => r.PatientId == patientId && r.Id == id);

        if (recommendation == null)
        {
            return NotFound();
        }

        return _mapper.Map<PatientRecommendationDto>(recommendation);
    }

    [HttpPost]
    public async Task<ActionResult<PatientRecommendationDto>> CreateRecommendation(
        string patientId,
        PatientRecommendationCreateDto recommendationDto)
    {
        var patient = await _context.Patients.FindAsync(patientId);
        if (patient == null)
        {
            return NotFound();
        }

        var recommendation = _mapper.Map<PatientRecommendation>(recommendationDto);
        recommendation.Id = Guid.NewGuid().ToString();
        recommendation.PatientId = patientId;
        recommendation.CreatedAt = DateTime.UtcNow;
        recommendation.UpdatedAt = DateTime.UtcNow;

        // Set creator info from current user if not provided
        if (string.IsNullOrEmpty(recommendation.CreatedById))
        {
            recommendation.CreatedById = GetCurrentUserId();
        }

        if (string.IsNullOrEmpty(recommendation.CreatedByName))
        {
            recommendation.CreatedByName = GetCurrentUserName();
        }

        _context.PatientRecommendations.Add(recommendation);
        await _context.SaveChangesAsync();

        // Publish event
        await _eventPublisher.PublishPatientRecommendationAddedEventAsync(recommendation);

        var resultDto = _mapper.Map<PatientRecommendationDto>(recommendation);
        return CreatedAtAction(
            nameof(GetRecommendation),
            new { patientId = patientId, id = recommendation.Id },
            resultDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRecommendation(
        string patientId,
        string id,
        PatientRecommendationUpdateDto recommendationDto)
    {
        var existingRecommendation = await _context.PatientRecommendations
            .FirstOrDefaultAsync(r => r.PatientId == patientId && r.Id == id);

        if (existingRecommendation == null)
        {
            return NotFound();
        }

        // Update properties using AutoMapper
        _mapper.Map(recommendationDto, existingRecommendation);
        existingRecommendation.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!RecommendationExists(id))
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
    public async Task<IActionResult> DeleteRecommendation(string patientId, string id)
    {
        var recommendation = await _context.PatientRecommendations
            .FirstOrDefaultAsync(r => r.PatientId == patientId && r.Id == id);

        if (recommendation == null)
        {
            return NotFound();
        }

        _context.PatientRecommendations.Remove(recommendation);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool RecommendationExists(string id)
    {
        return _context.PatientRecommendations.Any(e => e.Id == id);
    }

    // Helper methods to get user info from claims
    private string GetCurrentUserId()
    {
        return User.FindFirst("sub")?.Value ?? "system";
    }

    private string GetCurrentUserName()
    {
        return User.FindFirst("name")?.Value ?? "System";
    }
}