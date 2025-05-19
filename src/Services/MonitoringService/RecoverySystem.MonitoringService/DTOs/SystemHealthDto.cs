using System;
using System.Collections.Generic;

namespace RecoverySystem.MonitoringService.DTOs
{
    public class SystemHealthDto
    {
        public Guid Id { get; set; }
        public string ServiceName { get; set; }
        public string Status { get; set; }
        public Dictionary<string, string> Components { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class CreateSystemHealthDto
    {
        public string ServiceName { get; set; }
        public string Status { get; set; }
        public Dictionary<string, string> Components { get; set; }
        public string Description { get; set; }
    }
}