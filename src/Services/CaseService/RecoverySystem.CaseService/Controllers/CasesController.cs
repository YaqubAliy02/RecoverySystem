// Controllers/CasesController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecoverySystem.CaseService.Data;
using RecoverySystem.CaseService.Models;

namespace RecoverySystem.CaseService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CasesController : ControllerBase
    {
        private readonly CaseDbContext _context;
        private readonly ILogger<CasesController> _logger;

        public CasesController(CaseDbContext context, ILogger<CasesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Case>>> GetCases()
        {
            return await _context.Cases
                .Include(c => c.TimelineEvents.OrderByDescending(t => t.Timestamp).Take(3))
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Case>> GetCase(string id)
        {
            var @case = await _context.Cases
                .Include(c => c.Notes.OrderByDescending(n => n.CreatedAt))
                .Include(c => c.Documents.OrderByDescending(d => d.UploadedAt))
                .Include(c => c.TimelineEvents.OrderByDescending(t => t.Timestamp))
                .FirstOrDefaultAsync(c => c.Id == id);

            if (@case == null)
            {
                return NotFound();
            }

            return @case;
        }

        [HttpPost]
        public async Task<ActionResult<Case>> CreateCase(Case @case)
        {
            @case.Id = Guid.NewGuid().ToString();
            @case.CaseNumber = GenerateCaseNumber();
            @case.CreatedAt = DateTime.UtcNow;
            @case.UpdatedAt = DateTime.UtcNow;

            // Add initial timeline event
            var timelineEvent = new CaseTimelineEvent
            {
                CaseId = @case.Id,
                EventType = "created",
                Description = "Case was created",
                Timestamp = DateTime.UtcNow,
                UserId = GetCurrentUserId(),
                UserName = GetCurrentUserName(),
                UserAvatar = GetCurrentUserAvatar()
            };

            @case.TimelineEvents.Add(timelineEvent);

            _context.Cases.Add(@case);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCase), new { id = @case.Id }, @case);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCase(string id, Case @case)
        {
            if (id != @case.Id)
            {
                return BadRequest();
            }

            var existingCase = await _context.Cases.FindAsync(id);
            if (existingCase == null)
            {
                return NotFound();
            }

            // Update only allowed fields
            existingCase.Title = @case.Title;
            existingCase.Description = @case.Description;
            existingCase.Status = @case.Status;
            existingCase.Priority = @case.Priority;
            existingCase.AssignedToId = @case.AssignedToId;
            existingCase.AssignedToName = @case.AssignedToName;
            existingCase.AssignedToAvatar = @case.AssignedToAvatar;
            existingCase.Category = @case.Category;
            existingCase.Type = @case.Type;
            existingCase.DueDate = @case.DueDate;
            existingCase.UpdatedAt = DateTime.UtcNow;

            // If status changed to closed, set ClosedAt
            if (@case.Status == "closed" && existingCase.Status != "closed")
            {
                existingCase.ClosedAt = DateTime.UtcNow;

                // Add timeline event for case closure
                var timelineEvent = new CaseTimelineEvent
                {
                    CaseId = id,
                    EventType = "closed",
                    Description = "Case was closed",
                    Timestamp = DateTime.UtcNow,
                    UserId = GetCurrentUserId(),
                    UserName = GetCurrentUserName(),
                    UserAvatar = GetCurrentUserAvatar()
                };

                _context.CaseTimelineEvents.Add(timelineEvent);
            }
            else if (@case.Status != "closed" && existingCase.Status == "closed")
            {
                existingCase.ClosedAt = null;

                // Add timeline event for case reopening
                var timelineEvent = new CaseTimelineEvent
                {
                    CaseId = id,
                    EventType = "reopened",
                    Description = "Case was reopened",
                    Timestamp = DateTime.UtcNow,
                    UserId = GetCurrentUserId(),
                    UserName = GetCurrentUserName(),
                    UserAvatar = GetCurrentUserAvatar()
                };

                _context.CaseTimelineEvents.Add(timelineEvent);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CaseExists(id))
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

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateCaseStatus(string id, [FromBody] UpdateStatusRequest request)
        {
            var @case = await _context.Cases.FindAsync(id);
            if (@case == null)
            {
                return NotFound();
            }

            var oldStatus = @case.Status;
            @case.Status = request.Status;
            @case.UpdatedAt = DateTime.UtcNow;

            // If status changed to closed, set ClosedAt
            if (request.Status == "closed" && oldStatus != "closed")
            {
                @case.ClosedAt = DateTime.UtcNow;
            }
            else if (request.Status != "closed" && oldStatus == "closed")
            {
                @case.ClosedAt = null;
            }

            // Add timeline event for status change
            var timelineEvent = new CaseTimelineEvent
            {
                CaseId = id,
                EventType = "status_changed",
                Description = $"Status changed from {oldStatus} to {request.Status}",
                Timestamp = DateTime.UtcNow,
                UserId = GetCurrentUserId(),
                UserName = GetCurrentUserName(),
                UserAvatar = GetCurrentUserAvatar(),
                Metadata = $"{{\"oldStatus\":\"{oldStatus}\",\"newStatus\":\"{request.Status}\"}}"
            };

            _context.CaseTimelineEvents.Add(timelineEvent);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCase(string id)
        {
            var @case = await _context.Cases.FindAsync(id);
            if (@case == null)
            {
                return NotFound();
            }

            _context.Cases.Remove(@case);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/notes")]
        public async Task<ActionResult<CaseNote>> AddNote(string id, [FromBody] AddNoteRequest request)
        {
            var @case = await _context.Cases.FindAsync(id);
            if (@case == null)
            {
                return NotFound();
            }

            var note = new CaseNote
            {
                Id = Guid.NewGuid().ToString(),
                CaseId = id,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow,
                AuthorId = GetCurrentUserId(),
                AuthorName = GetCurrentUserName(),
                AuthorAvatar = GetCurrentUserAvatar(),
                AuthorRole = GetCurrentUserRole()
            };

            _context.CaseNotes.Add(note);

            // Add timeline event for note addition
            var timelineEvent = new CaseTimelineEvent
            {
                CaseId = id,
                EventType = "note_added",
                Description = "Note was added to the case",
                Timestamp = DateTime.UtcNow,
                UserId = GetCurrentUserId(),
                UserName = GetCurrentUserName(),
                UserAvatar = GetCurrentUserAvatar()
            };

            _context.CaseTimelineEvents.Add(timelineEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCase), new { id = id }, note);
        }

        [HttpPost("{id}/documents")]
        public async Task<ActionResult<CaseDocument>> AddDocument(string id, [FromForm] AddDocumentRequest request)
        {
            var @case = await _context.Cases.FindAsync(id);
            if (@case == null)
            {
                return NotFound();
            }

            // In a real implementation, you would upload the file to a storage service
            // and get back a URL. For this example, we'll just create a placeholder URL.
            string fileUrl = $"/api/documents/{Guid.NewGuid()}";

            var document = new CaseDocument
            {
                Id = Guid.NewGuid().ToString(),
                CaseId = id,
                FileName = request.File.FileName,
                FileType = request.File.ContentType,
                FileSize = $"{request.File.Length / 1024} KB",
                FileUrl = fileUrl,
                UploadedAt = DateTime.UtcNow,
                UploadedById = GetCurrentUserId(),
                UploadedByName = GetCurrentUserName(),
                UploadedByAvatar = GetCurrentUserAvatar(),
                Description = request.Description
            };

            _context.CaseDocuments.Add(document);

            // Add timeline event for document upload
            var timelineEvent = new CaseTimelineEvent
            {
                CaseId = id,
                EventType = "document_uploaded",
                Description = $"Document '{request.File.FileName}' was uploaded",
                Timestamp = DateTime.UtcNow,
                UserId = GetCurrentUserId(),
                UserName = GetCurrentUserName(),
                UserAvatar = GetCurrentUserAvatar(),
                Metadata = $"{{\"fileName\":\"{request.File.FileName}\",\"fileSize\":\"{document.FileSize}\"}}"
            };

            _context.CaseTimelineEvents.Add(timelineEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCase), new { id = id }, document);
        }

        [HttpGet("by-patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<Case>>> GetCasesByPatient(string patientId)
        {
            return await _context.Cases
                .Where(c => c.PatientId == patientId)
                .ToListAsync();
        }

        [HttpGet("by-status/{status}")]
        public async Task<ActionResult<IEnumerable<Case>>> GetCasesByStatus(string status)
        {
            return await _context.Cases
                .Where(c => c.Status == status)
                .ToListAsync();
        }

        [HttpGet("by-priority/{priority}")]
        public async Task<ActionResult<IEnumerable<Case>>> GetCasesByPriority(string priority)
        {
            return await _context.Cases
                .Where(c => c.Priority == priority)
                .ToListAsync();
        }

        [HttpGet("by-assigned/{userId}")]
        public async Task<ActionResult<IEnumerable<Case>>> GetCasesByAssignedUser(string userId)
        {
            return await _context.Cases
                .Where(c => c.AssignedToId == userId)
                .ToListAsync();
        }

        private bool CaseExists(string id)
        {
            return _context.Cases.Any(e => e.Id == id);
        }

        private string GenerateCaseNumber()
        {
            // Generate a unique case number with format: CS-YYYYMMDD-XXXX
            string dateComponent = DateTime.UtcNow.ToString("yyyyMMdd");
            int count = _context.Cases.Count() + 1;
            return $"CS-{dateComponent}-{count:D4}";
        }

        // These methods would normally get the user info from the JWT token
        // For simplicity, we're using placeholder implementations
        private string GetCurrentUserId()
        {
            // In a real implementation, get this from the JWT token
            return User.FindFirst("sub")?.Value ?? "system";
        }

        private string GetCurrentUserName()
        {
            // In a real implementation, get this from the JWT token
            return User.FindFirst("name")?.Value ?? "System";
        }

        private string GetCurrentUserAvatar()
        {
            // In a real implementation, get this from the JWT token or user profile
            return "/placeholder.svg?height=40&width=40";
        }

        private string GetCurrentUserRole()
        {
            // In a real implementation, get this from the JWT token
            return User.FindFirst("role")?.Value ?? "user";
        }
    }

    public class UpdateStatusRequest
    {
        public string Status { get; set; }
    }

    public class AddNoteRequest
    {
        public string Content { get; set; }
    }

    public class AddDocumentRequest
    {
        public IFormFile File { get; set; }
        public string Description { get; set; }
    }
}