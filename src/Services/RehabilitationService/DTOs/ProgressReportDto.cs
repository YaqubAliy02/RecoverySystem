using RecoverySystem.RehabilitationService.Models;
using System;
using System.Collections.Generic;

namespace RecoverySystem.RehabilitationService.DTOs
{
    public class ProgressReportDto
    {
        public Guid Id { get; set; }
        public Guid RehabilitationProgramId { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }
        public DateTime ReportDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalSessions { get; set; }
        public int CompletedSessions { get; set; }
        public int MissedSessions { get; set; }
        public double ComplianceRate { get; set; }
        public List<ActivityProgressDto> ActivityProgress { get; set; }
        public string Assessment { get; set; }
        public string Recommendations { get; set; }
        public Guid CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ActivityProgressDto
    {
        public Guid ActivityId { get; set; }
        public string ActivityTitle { get; set; }
        public int TotalCompletions { get; set; }
        public int ExpectedCompletions { get; set; }
        public double ComplianceRate { get; set; }
        public string Notes { get; set; }
    }

    public class CreateProgressReportDto
    {
        public Guid RehabilitationProgramId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Assessment { get; set; }
        public string Recommendations { get; set; }
    }
}