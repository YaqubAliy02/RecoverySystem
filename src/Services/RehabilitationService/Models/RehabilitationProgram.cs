using System;
using System.Collections.Generic;

namespace RecoverySystem.RehabilitationService.Models
{
    public class RehabilitationProgram
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }
        public Guid CaseId { get; set; }
        public Guid AssignedToId { get; set; }
        public string AssignedToName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public RehabilitationStatus Status { get; set; } = RehabilitationStatus.Planned;
        public List<RehabilitationActivity> Activities { get; set; } = new List<RehabilitationActivity>();
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public enum RehabilitationStatus
    {
        Planned,
        InProgress,
        Completed,
        Cancelled
    }
}