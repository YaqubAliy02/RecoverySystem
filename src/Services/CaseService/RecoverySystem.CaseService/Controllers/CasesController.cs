using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecoverySystem.CaseService.DTOs;
using RecoverySystem.CaseService.Models;
using RecoverySystem.CaseService.Services;

namespace RecoverySystem.CaseService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CasesController : ControllerBase
{
    private readonly ICaseService _caseService;
    private readonly ILogger<CasesController> _logger;

    public CasesController(ICaseService caseService, ILogger<CasesController> logger)
    {
        _caseService = caseService ?? throw new ArgumentNullException(nameof(caseService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CaseDto>>> GetAllCases()
    {
        _logger.LogInformation("Getting all cases");
        var cases = await _caseService.GetAllCasesAsync();
        return Ok(cases);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CaseDto>> GetCaseById(Guid id)
    {
        _logger.LogInformation("Getting case with ID {CaseId}", id);
        var @case = await _caseService.GetCaseByIdAsync(id);

        if (@case == null)
        {
            return NotFound();
        }

        return Ok(@case);
    }

    [HttpPost]
    public async Task<ActionResult<CaseDto>> CreateCase(CreateCaseDto createCaseDto)
    {
        _logger.LogInformation("Creating a new case");

        var currentUserId = GetCurrentUserId();
        var createdCase = await _caseService.CreateCaseAsync(createCaseDto, currentUserId);

        return CreatedAtAction(nameof(GetCaseById), new { id = createdCase.Id }, createdCase);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CaseDto>> UpdateCase(Guid id, UpdateCaseDto updateCaseDto)
    {
        _logger.LogInformation("Updating case with ID {CaseId}", id);

        var updatedCase = await _caseService.UpdateCaseAsync(id, updateCaseDto);

        if (updatedCase == null)
        {
            return NotFound();
        }

        return Ok(updatedCase);
    }

    [HttpPatch("{id}/status")]
    public async Task<ActionResult<CaseDto>> UpdateCaseStatus(Guid id, UpdateCaseStatusDto updateStatusDto)
    {
        _logger.LogInformation("Updating status for case with ID {CaseId}", id);

        var updatedCase = await _caseService.UpdateCaseStatusAsync(id, updateStatusDto);

        if (updatedCase == null)
        {
            return NotFound();
        }

        return Ok(updatedCase);
    }

    [HttpGet("{id}/notes")]
    public async Task<ActionResult<IEnumerable<CaseNoteDto>>> GetCaseNotes(Guid id)
    {
        _logger.LogInformation("Getting notes for case with ID {CaseId}", id);

        var notes = await _caseService.GetCaseNotesAsync(id);
        return Ok(notes);
    }

    [HttpPost("{id}/notes")]
    public async Task<ActionResult<CaseNoteDto>> AddCaseNote(Guid id, CreateCaseNoteDto createNoteDto)
    {
        _logger.LogInformation("Adding note to case with ID {CaseId}", id);

        var currentUserId = GetCurrentUserId();
        var createdNote = await _caseService.AddCaseNoteAsync(id, createNoteDto, currentUserId);

        if (createdNote == null)
        {
            return NotFound();
        }

        return CreatedAtAction(nameof(GetCaseNotes), new { id }, createdNote);
    }

    [HttpGet("patient/{patientId}")]
    public async Task<ActionResult<IEnumerable<CaseDto>>> GetCasesByPatientId(Guid patientId)
    {
        _logger.LogInformation("Getting cases for patient with ID {PatientId}", patientId);

        var cases = await _caseService.GetCasesByPatientIdAsync(patientId);
        return Ok(cases);
    }

    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<CaseDto>>> GetCasesByStatus(string status)
    {
        _logger.LogInformation("Getting cases with status {Status}", status);

        if (!Enum.TryParse<CaseStatus>(status, true, out var caseStatus))
        {
            return BadRequest($"Invalid status value: {status}");
        }

        var cases = await _caseService.GetCasesByStatusAsync(caseStatus);
        return Ok(cases);
    }

    [HttpGet("assigned/{assignedToId}")]
    public async Task<ActionResult<IEnumerable<CaseDto>>> GetCasesByAssignedToId(Guid assignedToId)
    {
        _logger.LogInformation("Getting cases assigned to user with ID {UserId}", assignedToId);

        var cases = await _caseService.GetCasesByAssignedToIdAsync(assignedToId);
        return Ok(cases);
    }

    [HttpPost("{id}/documents")]
    public async Task<ActionResult<CaseDocumentDto>> UploadDocument(Guid id, IFormFile file)
    {
        _logger.LogInformation("Uploading document for case with ID {CaseId}", id);

        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        // TODO: Implement file storage logic (e.g., save to disk, S3, Azure Blob Storage)
        // For now, we'll just create a document record with placeholder file path

        var document = new CaseDocument
        {
            FileName = file.FileName,
            ContentType = file.ContentType,
            FileSize = file.Length,
            FilePath = $"/documents/{Guid.NewGuid()}_{file.FileName}" // Placeholder path
        };

        var currentUserId = GetCurrentUserId();
        var uploadedDocument = await _caseService.UploadDocumentAsync(id, document, currentUserId);

        if (uploadedDocument == null)
        {
            return NotFound();
        }

        return Ok(uploadedDocument);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("Unable to get current user ID from claims");
            throw new InvalidOperationException("Unable to get current user ID from claims");
        }

        return userId;
    }
}