using System;

namespace RecoverySystem.CaseService.DTOs
{
    public class CaseDocumentDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
        public Guid UploadedById { get; set; }
        public string UploadedByName { get; set; }
    }
}