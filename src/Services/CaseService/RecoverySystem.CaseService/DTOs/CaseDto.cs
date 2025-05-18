using System;
using System.Collections.Generic;
using RecoverySystem.CaseService.Models;

namespace RecoverySystem.CaseService.DTOs
{
    public class CaseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }

        public Guid PatientId { get; set; }
        public string PatientName { get; set; }

        public Guid AssignedToId { get; set; }
        public string AssignedToName { get; set; }

        public Guid CreatedById { get; set; }
        public string CreatedByName { get; set; }

        public ICollection<CaseNoteDto> Notes { get; set; }
        public ICollection<CaseDocumentDto> Documents { get; set; }
    }
}