using System;
using RecoverySystem.MonitoringService.Models;

namespace RecoverySystem.MonitoringService.DTOs
{
    public class ThresholdConfigurationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string VitalSign { get; set; }
        public double LowerThreshold { get; set; }
        public double UpperThreshold { get; set; }
        public AlertSeverity Severity { get; set; }
        public bool IsGlobal { get; set; }
        public Guid? PatientId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateThresholdConfigurationDto
    {
        public string Name { get; set; }
        public string VitalSign { get; set; }
        public double LowerThreshold { get; set; }
        public double UpperThreshold { get; set; }
        public AlertSeverity Severity { get; set; }
        public bool IsGlobal { get; set; } = true;
        public Guid? PatientId { get; set; }
    }

    public class UpdateThresholdConfigurationDto
    {
        public string Name { get; set; }
        public double LowerThreshold { get; set; }
        public double UpperThreshold { get; set; }
        public AlertSeverity Severity { get; set; }
        public bool IsActive { get; set; }
    }
}