using RecoverySystem.RehabilitationService.Models;
using System;
using System.Collections.Generic;

namespace RecoverySystem.RehabilitationService.DTOs
{
    public class RehabilitationProgramDto
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
        public RehabilitationStatus Status { get; set; }
        public List<RehabilitationActivityDto> Activities { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateRehabilitationProgramDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }
        public Guid? CaseId { get; set; }
        public Guid AssignedToId { get; set; }
        public string AssignedToName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Notes { get; set; }
    }

    public class UpdateRehabilitationProgramDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid AssignedToId { get; set; }
        public string AssignedToName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public RehabilitationStatus Status { get; set; }
        public string Notes { get; set; }
    }
}