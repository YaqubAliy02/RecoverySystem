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
    public class MonitoringService : IMonitoringService
    {
        private readonly MonitoringDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<MonitoringService> _logger;
        private readonly EventPublisher _eventPublisher;
        private readonly IAlertService _alertService;

        public MonitoringService(
            MonitoringDbContext dbContext,
            IMapper mapper,
            ILogger<MonitoringService> logger,
            EventPublisher eventPublisher,
            IAlertService alertService)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
            _alertService = alertService ?? throw new ArgumentNullException(nameof(alertService));
        }

        #region Vital Monitoring

        public async Task<IEnumerable<VitalMonitoringDto>> GetVitalMonitoringsForPatientAsync(Guid patientId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbContext.VitalMonitorings
                .Where(v => v.PatientId == patientId);

            if (startDate.HasValue)
            {
                query = query.Where(v => v.Timestamp >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(v => v.Timestamp <= endDate.Value);
            }

            var vitalMonitorings = await query
                .OrderByDescending(v => v.Timestamp)
                .ToListAsync();

            return _mapper.Map<IEnumerable<VitalMonitoringDto>>(vitalMonitorings);
        }

        public async Task<VitalMonitoringDto> GetVitalMonitoringByIdAsync(Guid id)
        {
            var vitalMonitoring = await _dbContext.VitalMonitorings.FindAsync(id);
            if (vitalMonitoring == null)
            {
                _logger.LogWarning("Vital monitoring with ID {Id} not found", id);
                return null;
            }

            return _mapper.Map<VitalMonitoringDto>(vitalMonitoring);
        }

        public async Task<VitalMonitoringDto> RecordVitalMonitoringAsync(CreateVitalMonitoringDto createDto, Guid currentUserId, string currentUserName)
        {
            var vitalMonitoring = _mapper.Map<VitalMonitoring>(createDto);
            vitalMonitoring.Id = Guid.NewGuid();
            vitalMonitoring.Timestamp = DateTime.UtcNow;
            vitalMonitoring.RecordedById = currentUserId;
            vitalMonitoring.RecordedByName = currentUserName;

            await _dbContext.VitalMonitorings.AddAsync(vitalMonitoring);
            await _dbContext.SaveChangesAsync();

            // Publish event
            try
            {
                await _eventPublisher.PublishVitalSignsRecordedEventAsync(vitalMonitoring);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish VitalSignsRecordedEvent for monitoring {Id}", vitalMonitoring.Id);
            }

            // Check thresholds and create alerts if needed
            await CheckVitalSignsThresholdsAsync(vitalMonitoring);

            return _mapper.Map<VitalMonitoringDto>(vitalMonitoring);
        }

        public async Task<IEnumerable<VitalMonitoringDto>> GetLatestVitalMonitoringsAsync(int count = 10)
        {
            var vitalMonitorings = await _dbContext.VitalMonitorings
                .OrderByDescending(v => v.Timestamp)
                .Take(count)
                .ToListAsync();

            return _mapper.Map<IEnumerable<VitalMonitoringDto>>(vitalMonitorings);
        }

        private async Task CheckVitalSignsThresholdsAsync(VitalMonitoring vitalMonitoring)
        {
            // Get patient-specific thresholds first, then global thresholds
            var thresholds = await _dbContext.ThresholdConfigurations
                .Where(t => t.IsActive &&
                           ((t.PatientId == vitalMonitoring.PatientId) ||
                            (t.IsGlobal && t.PatientId == null)))
                .ToListAsync();

            foreach (var threshold in thresholds)
            {
                bool thresholdExceeded = false;
                string message = string.Empty;
                double value = 0;

                switch (threshold.VitalSign.ToLower())
                {
                    case "heartrate":
                        value = vitalMonitoring.HeartRate;
                        thresholdExceeded = vitalMonitoring.HeartRate < threshold.LowerThreshold ||
                                           vitalMonitoring.HeartRate > threshold.UpperThreshold;
                        message = $"Heart rate {vitalMonitoring.HeartRate} is outside normal range ({threshold.LowerThreshold}-{threshold.UpperThreshold})";
                        break;
                    case "temperature":
                        value = vitalMonitoring.Temperature;
                        thresholdExceeded = vitalMonitoring.Temperature < threshold.LowerThreshold ||
                                           vitalMonitoring.Temperature > threshold.UpperThreshold;
                        message = $"Temperature {vitalMonitoring.Temperature} is outside normal range ({threshold.LowerThreshold}-{threshold.UpperThreshold})";
                        break;
                    case "respiratoryrate":
                        value = vitalMonitoring.RespiratoryRate;
                        thresholdExceeded = vitalMonitoring.RespiratoryRate < threshold.LowerThreshold ||
                                           vitalMonitoring.RespiratoryRate > threshold.UpperThreshold;
                        message = $"Respiratory rate {vitalMonitoring.RespiratoryRate} is outside normal range ({threshold.LowerThreshold}-{threshold.UpperThreshold})";
                        break;
                    case "oxygensaturation":
                        value = vitalMonitoring.OxygenSaturation;
                        thresholdExceeded = vitalMonitoring.OxygenSaturation < threshold.LowerThreshold ||
                                           vitalMonitoring.OxygenSaturation > threshold.UpperThreshold;
                        message = $"Oxygen saturation {vitalMonitoring.OxygenSaturation}% is outside normal range ({threshold.LowerThreshold}-{threshold.UpperThreshold})";
                        break;
                    case "painlevel":
                        value = vitalMonitoring.PainLevel;
                        thresholdExceeded = vitalMonitoring.PainLevel > threshold.UpperThreshold;
                        message = $"Pain level {vitalMonitoring.PainLevel} exceeds threshold ({threshold.UpperThreshold})";
                        break;
                }

                if (thresholdExceeded)
                {
                    var alertDto = new CreateAlertDto
                    {
                        PatientId = vitalMonitoring.PatientId,
                        PatientName = "Patient", // In a real system, you'd get the patient name
                        Type = AlertType.VitalSigns,
                        Severity = threshold.Severity,
                        Message = message,
                        Details = $"Recorded value: {value}. Threshold: {threshold.LowerThreshold}-{threshold.UpperThreshold}. Recorded at {vitalMonitoring.Timestamp}."
                    };

                    await _alertService.CreateAlertAsync(alertDto);
                }
            }
        }

        #endregion

        #region Alerts

        public async Task<IEnumerable<AlertDto>> GetAlertsAsync(bool includeResolved = false, AlertSeverity? severity = null)
        {
            var query = _dbContext.Alerts.AsQueryable();

            if (!includeResolved)
            {
                query = query.Where(a => !a.IsResolved);
            }

            if (severity.HasValue)
            {
                query = query.Where(a => a.Severity == severity.Value);
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

        #endregion

        #region System Health

        public async Task<IEnumerable<SystemHealthDto>> GetSystemHealthsAsync()
        {
            var systemHealths = await _dbContext.SystemHealths
                .OrderByDescending(s => s.Timestamp)
                .ToListAsync();

            return _mapper.Map<IEnumerable<SystemHealthDto>>(systemHealths);
        }

        public async Task<SystemHealthDto> GetSystemHealthByServiceNameAsync(string serviceName)
        {
            var systemHealth = await _dbContext.SystemHealths
                .Where(s => s.ServiceName == serviceName)
                .OrderByDescending(s => s.Timestamp)
                .FirstOrDefaultAsync();

            if (systemHealth == null)
            {
                _logger.LogWarning("System health for service {ServiceName} not found", serviceName);
                return null;
            }

            return _mapper.Map<SystemHealthDto>(systemHealth);
        }

        public async Task<SystemHealthDto> RecordSystemHealthAsync(CreateSystemHealthDto createDto)
        {
            var systemHealth = _mapper.Map<SystemHealth>(createDto);
            systemHealth.Id = Guid.NewGuid();
            systemHealth.Timestamp = DateTime.UtcNow;

            await _dbContext.SystemHealths.AddAsync(systemHealth);
            await _dbContext.SaveChangesAsync();

            // Publish event if status is not healthy
            if (systemHealth.Status.ToLower() != "healthy")
            {
                try
                {
                    await _eventPublisher.PublishSystemHealthChangedEventAsync(systemHealth);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to publish SystemHealthChangedEvent for service {ServiceName}", systemHealth.ServiceName);
                }
            }

            return _mapper.Map<SystemHealthDto>(systemHealth);
        }

        #endregion

        #region Threshold Configuration

        public async Task<IEnumerable<ThresholdConfigurationDto>> GetThresholdConfigurationsAsync(bool includeInactive = false)
        {
            var query = _dbContext.ThresholdConfigurations.AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(t => t.IsActive);
            }

            var thresholds = await query
                .OrderBy(t => t.VitalSign)
                .ThenBy(t => t.Name)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ThresholdConfigurationDto>>(thresholds);
        }

        public async Task<IEnumerable<ThresholdConfigurationDto>> GetThresholdConfigurationsForPatientAsync(Guid patientId, bool includeInactive = false)
        {
            var query = _dbContext.ThresholdConfigurations
                .Where(t => t.PatientId == patientId || (t.IsGlobal && t.PatientId == null));

            if (!includeInactive)
            {
                query = query.Where(t => t.IsActive);
            }

            var thresholds = await query
                .OrderBy(t => t.VitalSign)
                .ThenBy(t => t.Name)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ThresholdConfigurationDto>>(thresholds);
        }

        public async Task<ThresholdConfigurationDto> GetThresholdConfigurationByIdAsync(Guid id)
        {
            var threshold = await _dbContext.ThresholdConfigurations.FindAsync(id);
            if (threshold == null)
            {
                _logger.LogWarning("Threshold configuration with ID {Id} not found", id);
                return null;
            }

            return _mapper.Map<ThresholdConfigurationDto>(threshold);
        }

        public async Task<ThresholdConfigurationDto> CreateThresholdConfigurationAsync(CreateThresholdConfigurationDto createDto)
        {
            var threshold = _mapper.Map<ThresholdConfiguration>(createDto);
            threshold.Id = Guid.NewGuid();
            threshold.CreatedAt = DateTime.UtcNow;

            await _dbContext.ThresholdConfigurations.AddAsync(threshold);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<ThresholdConfigurationDto>(threshold);
        }

        public async Task<ThresholdConfigurationDto> UpdateThresholdConfigurationAsync(Guid id, UpdateThresholdConfigurationDto updateDto)
        {
            var threshold = await _dbContext.ThresholdConfigurations.FindAsync(id);
            if (threshold == null)
            {
                _logger.LogWarning("Threshold configuration with ID {Id} not found", id);
                return null;
            }

            threshold.Name = updateDto.Name;
            threshold.LowerThreshold = updateDto.LowerThreshold;
            threshold.UpperThreshold = updateDto.UpperThreshold;
            threshold.Severity = updateDto.Severity;
            threshold.IsActive = updateDto.IsActive;
            threshold.UpdatedAt = DateTime.UtcNow;

            _dbContext.ThresholdConfigurations.Update(threshold);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<ThresholdConfigurationDto>(threshold);
        }

        public async Task<bool> DeleteThresholdConfigurationAsync(Guid id)
        {
            var threshold = await _dbContext.ThresholdConfigurations.FindAsync(id);
            if (threshold == null)
            {
                _logger.LogWarning("Threshold configuration with ID {Id} not found", id);
                return false;
            }

            _dbContext.ThresholdConfigurations.Remove(threshold);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        #endregion
    }
}