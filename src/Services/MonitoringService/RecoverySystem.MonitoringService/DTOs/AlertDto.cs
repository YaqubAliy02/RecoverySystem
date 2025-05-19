using System;
using RecoverySystem.MonitoringService.Models;

namespace RecoverySystem.MonitoringService.DTOs
{
    public class AlertDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }
        public AlertType Type { get; set; }
        public AlertSeverity Severity { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public bool IsResolved { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public Guid? ResolvedById { get; set; }
        public string ResolvedByName { get; set; }
        public string ResolutionNotes { get; set; }
    }

    public class CreateAlertDto
    {
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }
        public AlertType Type { get; set; }
        public AlertSeverity Severity { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
    }

    public class ResolveAlertDto
    {
        public string ResolutionNotes { get; set; }
    }
}