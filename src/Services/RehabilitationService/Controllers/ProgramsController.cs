using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecoverySystem.RehabilitationService.DTOs;
using RecoverySystem.RehabilitationService.Models;
using RecoverySystem.RehabilitationService.Services;

namespace RecoverySystem.RehabilitationService.Controllers;

[ApiController]
[Route("api/rehabilitation/programs")]
[Authorize]
public class ProgramsController : ControllerBase
{
    private readonly IRehabilitationService _rehabilitationService;
    private readonly ILogger<ProgramsController> _logger;

    public ProgramsController(
        IRehabilitationService rehabilitationService,
        ILogger<ProgramsController> logger)
    {
        _rehabilitationService = rehabilitationService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetPrograms([FromQuery] RehabilitationStatus? status = null)
    {
        var programs = await _rehabilitationService.GetRehabilitationProgramsAsync(status);
        return Ok(programs);
    }

    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetProgramsForPatient(Guid patientId)
    {
        var programs = await _rehabilitationService.GetRehabilitationProgramsForPatientAsync(patientId);
        return Ok(programs);
    }

    [HttpGet("case/{caseId}")]
    public async Task<IActionResult> GetProgramsForCase(Guid caseId)
    {
        var programs = await _rehabilitationService.GetRehabilitationProgramsForCaseAsync(caseId);
        return Ok(programs);
    }

    [HttpGet("assigned/{assignedToId}")]
    public async Task<IActionResult> GetProgramsAssignedTo(Guid assignedToId)
    {
        var programs = await _rehabilitationService.GetRehabilitationProgramsAssignedToAsync(assignedToId);
        return Ok(programs);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProgramById(Guid id)
    {
        var program = await _rehabilitationService.GetRehabilitationProgramByIdAsync(id);
        if (program == null)
        {
            return NotFound(new { message = "Rehabilitation program not found" });
        }

        return Ok(program);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProgram([FromBody] CreateRehabilitationProgramDto createDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var program = await _rehabilitationService.CreateRehabilitationProgramAsync(createDto);
        return CreatedAtAction(nameof(GetProgramById), new { id = program.Id }, program);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProgram(Guid id, [FromBody] UpdateRehabilitationProgramDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var program = await _rehabilitationService.UpdateRehabilitationProgramAsync(id, updateDto);
        if (program == null)
        {
            return NotFound(new { message = "Rehabilitation program not found" });
        }

        return Ok(program);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateProgramStatus(Guid id, [FromBody] UpdateStatusDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var program = await _rehabilitationService.UpdateRehabilitationProgramStatusAsync(id, updateDto.Status);
        if (program == null)
        {
            return NotFound(new { message = "Rehabilitation program not found" });
        }

        return Ok(program);
    }
}

public class UpdateStatusDto
{
    public RehabilitationStatus Status { get; set; }
}