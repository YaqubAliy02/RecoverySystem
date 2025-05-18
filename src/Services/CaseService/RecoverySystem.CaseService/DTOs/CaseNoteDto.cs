using System;

namespace RecoverySystem.CaseService.DTOs
{
    public class CaseNoteDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedById { get; set; }
        public string CreatedByName { get; set; }
    }
}