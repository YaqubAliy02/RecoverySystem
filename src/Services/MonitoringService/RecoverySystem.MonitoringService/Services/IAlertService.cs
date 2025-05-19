using RecoverySystem.MonitoringService.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecoverySystem.MonitoringService.Services
{
    public interface IAlertService
    {
        Task<IEnumerable<AlertDto>> GetAlertsAsync(bool includeResolved = false);
        Task<IEnumerable<AlertDto>> GetAlertsForPatientAsync(Guid patientId, bool includeResolved = false);
        Task<AlertDto> GetAlertByIdAsync(Guid id);
        Task<AlertDto> CreateAlertAsync(CreateAlertDto createDto);
        Task<AlertDto> ResolveAlertAsync(Guid id, ResolveAlertDto resolveDto, Guid currentUserId, string currentUserName);
    }
}