using System;
using RecoverySystem.CaseService.Models;

namespace RecoverySystem.CaseService.DTOs
{
    public class CreateCaseDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public CasePriority Priority { get; set; }
        public Guid PatientId { get; set; }
        public Guid AssignedToId { get; set; }
    }
}