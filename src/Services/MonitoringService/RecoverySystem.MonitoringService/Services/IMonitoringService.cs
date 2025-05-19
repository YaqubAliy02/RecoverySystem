using RecoverySystem.MonitoringService.DTOs;
using RecoverySystem.MonitoringService.Models;

namespace RecoverySystem.MonitoringService.Services;

public interface IMonitoringService
{
    // Vital Monitoring
    Task<IEnumerable<VitalMonitoringDto>> GetVitalMonitoringsForPatientAsync(Guid patientId, DateTime? startDate = null, DateTime? endDate = null);
    Task<VitalMonitoringDto> GetVitalMonitoringByIdAsync(Guid id);
    Task<VitalMonitoringDto> RecordVitalMonitoringAsync(CreateVitalMonitoringDto createDto, Guid currentUserId, string currentUserName);
    Task<IEnumerable<VitalMonitoringDto>> GetLatestVitalMonitoringsAsync(int count = 10);

    // Alerts
    Task<IEnumerable<AlertDto>> GetAlertsAsync(bool includeResolved = false, AlertSeverity? severity = null);
    Task<IEnumerable<AlertDto>> GetAlertsForPatientAsync(Guid patientId, bool includeResolved = false);
    Task<AlertDto> GetAlertByIdAsync(Guid id);
    Task<AlertDto> CreateAlertAsync(CreateAlertDto createDto);
    Task<AlertDto> ResolveAlertAsync(Guid id, ResolveAlertDto resolveDto, Guid currentUserId, string currentUserName);

    // System Health
    Task<IEnumerable<SystemHealthDto>> GetSystemHealthsAsync();
    Task<SystemHealthDto> GetSystemHealthByServiceNameAsync(string serviceName);
    Task<SystemHealthDto> RecordSystemHealthAsync(CreateSystemHealthDto createDto);

    // Threshold Configuration
    Task<IEnumerable<ThresholdConfigurationDto>> GetThresholdConfigurationsAsync(bool includeInactive = false);
    Task<IEnumerable<ThresholdConfigurationDto>> GetThresholdConfigurationsForPatientAsync(Guid patientId, bool includeInactive = false);
    Task<ThresholdConfigurationDto> GetThresholdConfigurationByIdAsync(Guid id);
    Task<ThresholdConfigurationDto> CreateThresholdConfigurationAsync(CreateThresholdConfigurationDto createDto);
    Task<ThresholdConfigurationDto> UpdateThresholdConfigurationAsync(Guid id, UpdateThresholdConfigurationDto updateDto);
    Task<bool> DeleteThresholdConfigurationAsync(Guid id);
}