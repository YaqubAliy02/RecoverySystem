using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecoverySystem.RehabilitationService.Data;
using RecoverySystem.RehabilitationService.DTOs;
using RecoverySystem.RehabilitationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecoverySystem.RehabilitationService.Services
{
    public class RehabilitationService : IRehabilitationService
    {
        private readonly RehabilitationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<RehabilitationService> _logger;
        private readonly EventPublisher _eventPublisher;

        public RehabilitationService(
            RehabilitationDbContext dbContext,
            IMapper mapper,
            ILogger<RehabilitationService> logger,
            EventPublisher eventPublisher)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        #region Rehabilitation Programs

        public async Task<IEnumerable<RehabilitationProgramDto>> GetRehabilitationProgramsAsync(RehabilitationStatus? status = null)
        {
            var query = _dbContext.RehabilitationPrograms.AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(p => p.Status == status.Value);
            }

            var programs = await query
                .Include(p => p.Activities)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<RehabilitationProgramDto>>(programs);
        }

        public async Task<IEnumerable<RehabilitationProgramDto>> GetRehabilitationProgramsForPatientAsync(Guid patientId)
        {
            var programs = await _dbContext.RehabilitationPrograms
                .Where(p => p.PatientId == patientId)
                .Include(p => p.Activities)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<RehabilitationProgramDto>>(programs);
        }

        public async Task<IEnumerable<RehabilitationProgramDto>> GetRehabilitationProgramsForCaseAsync(Guid caseId)
        {
            var programs = await _dbContext.RehabilitationPrograms
                .Where(p => p.CaseId == caseId)
                .Include(p => p.Activities)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<RehabilitationProgramDto>>(programs);
        }

        public async Task<IEnumerable<RehabilitationProgramDto>> GetRehabilitationProgramsAssignedToAsync(Guid assignedToId)
        {
            var programs = await _dbContext.RehabilitationPrograms
                .Where(p => p.AssignedToId == assignedToId)
                .Include(p => p.Activities)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<RehabilitationProgramDto>>(programs);
        }

        public async Task<RehabilitationProgramDto> GetRehabilitationProgramByIdAsync(Guid id)
        {
            var program = await _dbContext.RehabilitationPrograms
                .Include(p => p.Activities)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (program == null)
            {
                _logger.LogWarning("Rehabilitation program with ID {Id} not found", id);
                return null;
            }

            return _mapper.Map<RehabilitationProgramDto>(program);
        }

        public async Task<RehabilitationProgramDto> CreateRehabilitationProgramAsync(CreateRehabilitationProgramDto createDto)
        {
            var program = _mapper.Map<RehabilitationProgram>(createDto);
            program.Id = Guid.NewGuid();
            program.Status = RehabilitationStatus.Planned;
            program.CreatedAt = DateTime.UtcNow;

            await _dbContext.RehabilitationPrograms.AddAsync(program);
            await _dbContext.SaveChangesAsync();

            // Publish event
            try
            {
                await _eventPublisher.PublishRehabilitationStartedEventAsync(program);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish RehabilitationStartedEvent for program {Id}", program.Id);
            }

            return _mapper.Map<RehabilitationProgramDto>(program);
        }

        public async Task<RehabilitationProgramDto> UpdateRehabilitationProgramAsync(Guid id, UpdateRehabilitationProgramDto updateDto)
        {
            var program = await _dbContext.RehabilitationPrograms
                .Include(p => p.Activities)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (program == null)
            {
                _logger.LogWarning("Rehabilitation program with ID {Id} not found", id);
                return null;
            }

            var oldStatus = program.Status;
            _mapper.Map(updateDto, program);
            program.UpdatedAt = DateTime.UtcNow;

            _dbContext.RehabilitationPrograms.Update(program);
            await _dbContext.SaveChangesAsync();

            // Publish event if status changed
            if (oldStatus != program.Status)
            {
                try
                {
                    await _eventPublisher.PublishRehabilitationStatusChangedEventAsync(program, oldStatus);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to publish RehabilitationStatusChangedEvent for program {Id}", program.Id);
                }
            }

            return _mapper.Map<RehabilitationProgramDto>(program);
        }

        public async Task<RehabilitationProgramDto> UpdateRehabilitationProgramStatusAsync(Guid id, RehabilitationStatus status)
        {
            var program = await _dbContext.RehabilitationPrograms
                .Include(p => p.Activities)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (program == null)
            {
                _logger.LogWarning("Rehabilitation program with ID {Id} not found", id);
                return null;
            }

            var oldStatus = program.Status;
            program.Status = status;
            program.UpdatedAt = DateTime.UtcNow;

            if (status == RehabilitationStatus.Completed && !program.EndDate.HasValue)
            {
                program.EndDate = DateTime.UtcNow;
            }

            _dbContext.RehabilitationPrograms.Update(program);
            await _dbContext.SaveChangesAsync();

            // Publish event
            try
            {
                await _eventPublisher.PublishRehabilitationStatusChangedEventAsync(program, oldStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish RehabilitationStatusChangedEvent for program {Id}", program.Id);
            }

            return _mapper.Map<RehabilitationProgramDto>(program);
        }

        #endregion

        #region Rehabilitation Activities

        public async Task<IEnumerable<RehabilitationActivityDto>> GetActivitiesForProgramAsync(Guid programId)
        {
            var activities = await _dbContext.RehabilitationActivities
                .Where(a => a.RehabilitationProgramId == programId)
                .OrderBy(a => a.Title)
                .ToListAsync();

            return _mapper.Map<IEnumerable<RehabilitationActivityDto>>(activities);
        }

        public async Task<RehabilitationActivityDto> GetActivityByIdAsync(Guid id)
        {
            var activity = await _dbContext.RehabilitationActivities.FindAsync(id);
            if (activity == null)
            {
                _logger.LogWarning("Rehabilitation activity with ID {Id} not found", id);
                return null;
            }

            return _mapper.Map<RehabilitationActivityDto>(activity);
        }

        public async Task<RehabilitationActivityDto> CreateActivityAsync(CreateRehabilitationActivityDto createDto)
        {
            // Check if program exists
            var program = await _dbContext.RehabilitationPrograms.FindAsync(createDto.RehabilitationProgramId);
            if (program == null)
            {
                _logger.LogWarning("Rehabilitation program with ID {Id} not found", createDto.RehabilitationProgramId);
                return null;
            }

            var activity = _mapper.Map<RehabilitationActivity>(createDto);
            activity.Id = Guid.NewGuid();
            activity.CreatedAt = DateTime.UtcNow;

            await _dbContext.RehabilitationActivities.AddAsync(activity);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<RehabilitationActivityDto>(activity);
        }

        public async Task<RehabilitationActivityDto> UpdateActivityAsync(Guid id, UpdateRehabilitationActivityDto updateDto)
        {
            var activity = await _dbContext.RehabilitationActivities.FindAsync(id);
            if (activity == null)
            {
                _logger.LogWarning("Rehabilitation activity with ID {Id} not found", id);
                return null;
            }

            _mapper.Map(updateDto, activity);
            activity.UpdatedAt = DateTime.UtcNow;

            _dbContext.RehabilitationActivities.Update(activity);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<RehabilitationActivityDto>(activity);
        }

        public async Task<bool> DeleteActivityAsync(Guid id)
        {
            var activity = await _dbContext.RehabilitationActivities.FindAsync(id);
            if (activity == null)
            {
                _logger.LogWarning("Rehabilitation activity with ID {Id} not found", id);
                return false;
            }

            _dbContext.RehabilitationActivities.Remove(activity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        #endregion

        #region Rehabilitation Sessions

        public async Task<IEnumerable<RehabilitationSessionDto>> GetSessionsForProgramAsync(Guid programId)
        {
            var sessions = await _dbContext.RehabilitationSessions
                .Where(s => s.RehabilitationProgramId == programId)
                .OrderByDescending(s => s.ScheduledDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<RehabilitationSessionDto>>(sessions);
        }

        public async Task<RehabilitationSessionDto> GetSessionByIdAsync(Guid id)
        {
            var session = await _dbContext.RehabilitationSessions.FindAsync(id);
            if (session == null)
            {
                _logger.LogWarning("Rehabilitation session with ID {Id} not found", id);
                return null;
            }

            return _mapper.Map<RehabilitationSessionDto>(session);
        }

        public async Task<RehabilitationSessionDto> CreateSessionAsync(CreateRehabilitationSessionDto createDto)
        {
            // Check if program exists
            var program = await _dbContext.RehabilitationPrograms.FindAsync(createDto.RehabilitationProgramId);
            if (program == null)
            {
                _logger.LogWarning("Rehabilitation program with ID {Id} not found", createDto.RehabilitationProgramId);
                return null;
            }

            var session = _mapper.Map<RehabilitationSession>(createDto);
            session.Id = Guid.NewGuid();
            session.Status = SessionStatus.Scheduled;
            session.CreatedAt = DateTime.UtcNow;

            // Get activities for the program to initialize CompletedActivities
            var activities = await _dbContext.RehabilitationActivities
                .Where(a => a.RehabilitationProgramId == createDto.RehabilitationProgramId)
                .ToListAsync();

            session.CompletedActivities = activities.Select(a => new ActivityCompletion
            {
                ActivityId = a.Id,
                ActivityTitle = a.Title,
                IsCompleted = false
            }).ToList();

            await _dbContext.RehabilitationSessions.AddAsync(session);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<RehabilitationSessionDto>(session);
        }

        public async Task<RehabilitationSessionDto> UpdateSessionAsync(Guid id, UpdateRehabilitationSessionDto updateDto, Guid? supervisedById = null, string supervisedByName = null)
        {
            var session = await _dbContext.RehabilitationSessions.FindAsync(id);
            if (session == null)
            {
                _logger.LogWarning("Rehabilitation session with ID {Id} not found", id);
                return null;
            }

            var oldStatus = session.Status;
            _mapper.Map(updateDto, session);
            session.UpdatedAt = DateTime.UtcNow;

            if (supervisedById.HasValue)
            {
                session.SupervisedById = supervisedById;
                session.SupervisedByName = supervisedByName;
            }

            // If status changed to Completed, set CompletedDate if not already set
            if (oldStatus != SessionStatus.Completed && session.Status == SessionStatus.Completed && !session.CompletedDate.HasValue)
            {
                session.CompletedDate = DateTime.UtcNow;
            }

            _dbContext.RehabilitationSessions.Update(session);
            await _dbContext.SaveChangesAsync();

            // Publish event if session completed
            if (oldStatus != SessionStatus.Completed && session.Status == SessionStatus.Completed)
            {
                try
                {
                    await _eventPublisher.PublishRehabilitationSessionCompletedEventAsync(session);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to publish RehabilitationSessionCompletedEvent for session {Id}", session.Id);
                }
            }

            return _mapper.Map<RehabilitationSessionDto>(session);
        }

        public async Task<bool> DeleteSessionAsync(Guid id)
        {
            var session = await _dbContext.RehabilitationSessions.FindAsync(id);
            if (session == null)
            {
                _logger.LogWarning("Rehabilitation session with ID {Id} not found", id);
                return false;
            }

            _dbContext.RehabilitationSessions.Remove(session);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        #endregion

        #region Progress Reports

        public async Task<IEnumerable<ProgressReportDto>> GetProgressReportsForProgramAsync(Guid programId)
        {
            var reports = await _dbContext.ProgressReports
                .Where(r => r.RehabilitationProgramId == programId)
                .OrderByDescending(r => r.ReportDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProgressReportDto>>(reports);
        }

        public async Task<IEnumerable<ProgressReportDto>> GetProgressReportsForPatientAsync(Guid patientId)
        {
            var reports = await _dbContext.ProgressReports
                .Where(r => r.PatientId == patientId)
                .OrderByDescending(r => r.ReportDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProgressReportDto>>(reports);
        }

        public async Task<ProgressReportDto> GetProgressReportByIdAsync(Guid id)
        {
            var report = await _dbContext.ProgressReports.FindAsync(id);
            if (report == null)
            {
                _logger.LogWarning("Progress report with ID {Id} not found", id);
                return null;
            }

            return _mapper.Map<ProgressReportDto>(report);
        }

        public async Task<ProgressReportDto> CreateProgressReportAsync(CreateProgressReportDto createDto, Guid createdById, string createdByName)
        {
            // Check if program exists
            var program = await _dbContext.RehabilitationPrograms
                .Include(p => p.Activities)
                .FirstOrDefaultAsync(p => p.Id == createDto.RehabilitationProgramId);

            if (program == null)
            {
                _logger.LogWarning("Rehabilitation program with ID {Id} not found", createDto.RehabilitationProgramId);
                return null;
            }

            // Get sessions for the program within the date range
            var sessions = await _dbContext.RehabilitationSessions
                .Where(s => s.RehabilitationProgramId == createDto.RehabilitationProgramId &&
                           s.ScheduledDate >= createDto.StartDate &&
                           s.ScheduledDate <= createDto.EndDate)
                .ToListAsync();

            var report = _mapper.Map<ProgressReport>(createDto);
            report.Id = Guid.NewGuid();
            report.PatientId = program.PatientId;
            report.PatientName = program.PatientName;
            report.ReportDate = DateTime.UtcNow;
            report.TotalSessions = sessions.Count;
            report.CompletedSessions = sessions.Count(s => s.Status == SessionStatus.Completed);
            report.MissedSessions = sessions.Count(s => s.Status == SessionStatus.Missed);
            report.ComplianceRate = report.TotalSessions > 0
                ? (double)report.CompletedSessions / report.TotalSessions * 100
                : 0;
            report.CreatedById = createdById;
            report.CreatedByName = createdByName;
            report.CreatedAt = DateTime.UtcNow;

            // Calculate activity progress
            report.ActivityProgress = new List<ActivityProgress>();
            foreach (var activity in program.Activities)
            {
                // Calculate expected completions based on frequency and date range
                var daysDifference = (createDto.EndDate - createDto.StartDate).Days;
                var weeksInRange = Math.Ceiling(daysDifference / 7.0);
                var expectedCompletions = (int)(weeksInRange * activity.Frequency);

                // Count actual completions from sessions
                var totalCompletions = sessions
                    .Where(s => s.Status == SessionStatus.Completed)
                    .SelectMany(s => s.CompletedActivities)
                    .Count(a => a.ActivityId == activity.Id && a.IsCompleted);

                var complianceRate = expectedCompletions > 0
                    ? (double)totalCompletions / expectedCompletions * 100
                    : 0;

                report.ActivityProgress.Add(new ActivityProgress
                {
                    ActivityId = activity.Id,
                    ActivityTitle = activity.Title,
                    TotalCompletions = totalCompletions,
                    ExpectedCompletions = expectedCompletions,
                    ComplianceRate = complianceRate,
                    Notes = ""
                });
            }

            await _dbContext.ProgressReports.AddAsync(report);
            await _dbContext.SaveChangesAsync();

            // Publish event
            try
            {
                await _eventPublisher.PublishProgressReportCreatedEventAsync(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish ProgressReportCreatedEvent for report {Id}", report.Id);
            }

            return _mapper.Map<ProgressReportDto>(report);
        }

        #endregion
    }
}