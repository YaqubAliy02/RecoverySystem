// Controllers/DocumentsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecoverySystem.CaseService.Data;

namespace RecoverySystem.CaseService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DocumentsController : ControllerBase
    {
        private readonly CaseDbContext _context;
        private readonly ILogger<DocumentsController> _logger;
        private readonly IWebHostEnvironment _environment;

        public DocumentsController(
            CaseDbContext context,
            ILogger<DocumentsController> logger,
            IWebHostEnvironment environment)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDocument(string id)
        {
            var document = await _context.CaseDocuments.FindAsync(Guid.Parse(id));
            if (document == null)
                return NotFound();

            return Ok(new
            {
                document.Id,
                document.FileName,
                document.FilePath,
                document.ContentType,
                document.FileSize,
                document.UploadedAt
            });
        }

        [HttpPost]
        public async Task<IActionResult> UploadDocument([FromForm] UploadDocumentRequest request)
        {
            try
            {
                if (request.File == null || request.File.Length == 0)
                {
                    return BadRequest("No file was uploaded.");
                }

                // Create uploads directory if it doesn't exist
                string uploadsFolder = Path.Combine(_environment.ContentRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate a unique filename
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(request.File.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.File.CopyToAsync(stream);
                }

                // Generate a URL for the file
                string fileUrl = $"/api/documents/download/{uniqueFileName}";

                return Ok(new
                {
                    fileName = request.File.FileName,
                    fileType = request.File.ContentType,
                    fileSize = $"{request.File.Length / 1024} KB",
                    fileUrl = fileUrl
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading document");
                return StatusCode(500, "An error occurred while uploading the document.");
            }
        }

        [HttpGet("download/{fileName}")]
        public IActionResult DownloadDocument(string fileName)
        {
            try
            {
                string filePath = Path.Combine(_environment.ContentRootPath, "uploads", fileName);
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound();
                }

                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading document");
                return StatusCode(500, "An error occurred while downloading the document.");
            }
        }
    }

    public class UploadDocumentRequest
    {
        public IFormFile File { get; set; }
        public string Description { get; set; }
        public string CaseId { get; set; }
    }
}