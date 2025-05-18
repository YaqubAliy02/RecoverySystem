using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecoverySystem.CaseService.Data;
using RecoverySystem.CaseService.DTOs;
using RecoverySystem.CaseService.Models;

namespace RecoverySystem.CaseService.Services
{
    public class CaseService : ICaseService
    {
        private readonly CaseDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<CaseService> _logger;
        private readonly EventPublisher _eventPublisher;

        public CaseService(
            CaseDbContext dbContext,
            IMapper mapper,
            ILogger<CaseService> logger,
            EventPublisher eventPublisher)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public async Task<IEnumerable<CaseDto>> GetAllCasesAsync()
        {
            var cases = await _dbContext.Cases
                .Include(c => c.Notes)
                .Include(c => c.Documents)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CaseDto>>(cases);
        }

        public async Task<CaseDto> GetCaseByIdAsync(Guid id)
        {
            var @case = await _dbContext.Cases
                .Include(c => c.Notes)
                .Include(c => c.Documents)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (@case == null)
            {
                _logger.LogWarning("Case with ID {CaseId} not found", id);
                return null;
            }

            return _mapper.Map<CaseDto>(@case);
        }

        public async Task<CaseDto> CreateCaseAsync(CreateCaseDto createCaseDto, Guid currentUserId)
        {
            var @case = _mapper.Map<Case>(createCaseDto);
            @case.Id = Guid.NewGuid();
            @case.CreatedById = currentUserId;
            @case.Status = CaseStatus.Open;

            await _dbContext.Cases.AddAsync(@case);
            await _dbContext.SaveChangesAsync();

            // Publish event
            try
            {
                await _eventPublisher.PublishCaseCreatedEventAsync(@case);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish CaseCreatedEvent for case {CaseId}", @case.Id);
                // Continue execution - don't fail the API call if messaging fails
            }

            return _mapper.Map<CaseDto>(@case);
        }

        public async Task<CaseDto> UpdateCaseAsync(Guid id, UpdateCaseDto updateCaseDto)
        {
            var @case = await _dbContext.Cases
                .Include(c => c.Notes)
                .Include(c => c.Documents)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (@case == null)
            {
                _logger.LogWarning("Case with ID {CaseId} not found for update", id);
                return null;
            }

            _mapper.Map(updateCaseDto, @case);
            @case.UpdatedAt = DateTime.UtcNow;

            _dbContext.Cases.Update(@case);
            await _dbContext.SaveChangesAsync();

            // Publish event
            try
            {
                await _eventPublisher.PublishCaseUpdatedEventAsync(@case);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish CaseUpdatedEvent for case {CaseId}", @case.Id);
                // Continue execution - don't fail the API call if messaging fails
            }

            return _mapper.Map<CaseDto>(@case);
        }

        public async Task<CaseDto> UpdateCaseStatusAsync(Guid id, UpdateCaseStatusDto updateStatusDto)
        {
            var @case = await _dbContext.Cases
                .Include(c => c.Notes)
                .Include(c => c.Documents)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (@case == null)
            {
                _logger.LogWarning("Case with ID {CaseId} not found for status update", id);
                return null;
            }

            var oldStatus = @case.Status;
            @case.Status = updateStatusDto.Status;
            @case.UpdatedAt = DateTime.UtcNow;

            // If the case is being closed, set the ClosedAt timestamp
            if (updateStatusDto.Status == CaseStatus.Closed)
            {
                @case.ClosedAt = DateTime.UtcNow;
            }

            _dbContext.Cases.Update(@case);
            await _dbContext.SaveChangesAsync();

            // Publish event
            try
            {
                await _eventPublisher.PublishCaseStatusChangedEventAsync(@case, oldStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish CaseStatusChangedEvent for case {CaseId}", @case.Id);
                // Continue execution - don't fail the API call if messaging fails
            }

            return _mapper.Map<CaseDto>(@case);
        }

        public async Task<CaseNoteDto> AddCaseNoteAsync(Guid caseId, CreateCaseNoteDto createNoteDto, Guid currentUserId)
        {
            var @case = await _dbContext.Cases.FindAsync(caseId);
            if (@case == null)
            {
                _logger.LogWarning("Case with ID {CaseId} not found for adding note", caseId);
                return null;
            }

            var note = _mapper.Map<CaseNote>(createNoteDto);
            note.Id = Guid.NewGuid();
            note.CaseId = caseId;
            note.CreatedById = currentUserId;
            note.CreatedAt = DateTime.UtcNow;

            await _dbContext.CaseNotes.AddAsync(note);
            await _dbContext.SaveChangesAsync();

            // Publish event
            try
            {
                // In a real implementation, you would get the user's name from a user service or cache
                string createdByName = "User " + currentUserId.ToString();
                await _eventPublisher.PublishCaseNoteAddedEventAsync(note, createdByName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish CaseNoteAddedEvent for note {NoteId}", note.Id);
                // Continue execution - don't fail the API call if messaging fails
            }

            return _mapper.Map<CaseNoteDto>(note);
        }

        public async Task<IEnumerable<CaseNoteDto>> GetCaseNotesAsync(Guid caseId)
        {
            var notes = await _dbContext.CaseNotes
                .Where(n => n.CaseId == caseId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CaseNoteDto>>(notes);
        }

        public async Task<CaseDocumentDto> UploadDocumentAsync(Guid caseId, CaseDocument document, Guid currentUserId)
        {
            var @case = await _dbContext.Cases.FindAsync(caseId);
            if (@case == null)
            {
                _logger.LogWarning("Case with ID {CaseId} not found for document upload", caseId);
                return null;
            }

            document.Id = Guid.NewGuid();
            document.CaseId = caseId;
            document.UploadedById = currentUserId;
            document.UploadedAt = DateTime.UtcNow;

            await _dbContext.CaseDocuments.AddAsync(document);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<CaseDocumentDto>(document);
        }

        public async Task<IEnumerable<CaseDto>> GetCasesByPatientIdAsync(Guid patientId)
        {
            var cases = await _dbContext.Cases
                .Include(c => c.Notes)
                .Include(c => c.Documents)
                .Where(c => c.PatientId == patientId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CaseDto>>(cases);
        }

        public async Task<IEnumerable<CaseDto>> GetCasesByStatusAsync(CaseStatus status)
        {
            var cases = await _dbContext.Cases
                .Include(c => c.Notes)
                .Include(c => c.Documents)
                .Where(c => c.Status == status)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CaseDto>>(cases);
        }

        public async Task<IEnumerable<CaseDto>> GetCasesByAssignedToIdAsync(Guid assignedToId)
        {
            var cases = await _dbContext.Cases
                .Include(c => c.Notes)
                .Include(c => c.Documents)
                .Where(c => c.AssignedToId == assignedToId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CaseDto>>(cases);
        }
    }
}