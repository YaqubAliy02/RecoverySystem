using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecoverySystem.MonitoringService.Data;
using RecoverySystem.MonitoringService.DTOs;
using RecoverySystem.MonitoringService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecoverySystem.MonitoringService.Services
{
    public class AlertService : IAlertService
    {
        private readonly MonitoringDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<AlertService> _logger;
        private readonly EventPublisher _eventPublisher;

        public AlertService(
            MonitoringDbContext dbContext,
            IMapper mapper,
            ILogger<AlertService> logger,
            EventPublisher eventPublisher)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public async Task<IEnumerable<AlertDto>> GetAlertsAsync(bool includeResolved = false)
        {
            var query = _dbContext.Alerts.AsQueryable();

            if (!includeResolved)
            {
                query = query.Where(a => !a.IsResolved);
            }

            var alerts = await query
                .OrderByDescending(a => a.Severity)
                .ThenByDescending(a => a.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<AlertDto>>(alerts);
        }

        public async Task<IEnumerable<AlertDto>> GetAlertsForPatientAsync(Guid patientId, bool includeResolved = false)
        {
            var query = _dbContext.Alerts
                .Where(a => a.PatientId == patientId);

            if (!includeResolved)
            {
                query = query.Where(a => !a.IsResolved);
            }

            var alerts = await query
                .OrderByDescending(a => a.Severity)
                .ThenByDescending(a => a.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<AlertDto>>(alerts);
        }

        public async Task<AlertDto> GetAlertByIdAsync(Guid id)
        {
            var alert = await _dbContext.Alerts.FindAsync(id);
            if (alert == null)
            {
                _logger.LogWarning("Alert with ID {Id} not found", id);
                return null;
            }

            return _mapper.Map<AlertDto>(alert);
        }

        public async Task<AlertDto> CreateAlertAsync(CreateAlertDto createDto)
        {
            var alert = _mapper.Map<Alert>(createDto);
            alert.Id = Guid.NewGuid();
            alert.IsResolved = false;
            alert.CreatedAt = DateTime.UtcNow;

            await _dbContext.Alerts.AddAsync(alert);
            await _dbContext.SaveChangesAsync();

            // Publish event
            try
            {
                await _eventPublisher.PublishAlertCreatedEventAsync(alert);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish AlertCreatedEvent for alert {Id}", alert.Id);
            }

            return _mapper.Map<AlertDto>(alert);
        }

        public async Task<AlertDto> ResolveAlertAsync(Guid id, ResolveAlertDto resolveDto, Guid currentUserId, string currentUserName)
        {
            var alert = await _dbContext.Alerts.FindAsync(id);
            if (alert == null)
            {
                _logger.LogWarning("Alert with ID {Id} not found", id);
                return null;
            }

            alert.IsResolved = true;
            alert.ResolvedAt = DateTime.UtcNow;
            alert.ResolvedById = currentUserId;
            alert.ResolvedByName = currentUserName;
            alert.ResolutionNotes = resolveDto.ResolutionNotes;

            _dbContext.Alerts.Update(alert);
            await _dbContext.SaveChangesAsync();

            // Publish event
            try
            {
                await _eventPublisher.PublishAlertResolvedEventAsync(alert);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish AlertResolvedEvent for alert {Id}", alert.Id);
            }

            return _mapper.Map<AlertDto>(alert);
        }
    }
}