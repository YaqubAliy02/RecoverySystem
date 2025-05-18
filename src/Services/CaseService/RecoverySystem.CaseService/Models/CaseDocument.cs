// Models/CaseDocument.cs
using System;

namespace RecoverySystem.CaseService.Models
{
    public class CaseDocument
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CaseId { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FileSize { get; set; }
        public string FileUrl { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public string UploadedById { get; set; }
        public string UploadedByName { get; set; }
        public string UploadedByAvatar { get; set; }
        public string Description { get; set; }
        public Case Case { get; set; }
    }
}