// Models/CaseTimelineEvent.cs
using System;

namespace RecoverySystem.CaseService.Models
{
    public class CaseTimelineEvent
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CaseId { get; set; }
        public string EventType { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserAvatar { get; set; }
        public string Metadata { get; set; } // JSON string for additional data
        public Case Case { get; set; }
    }
}