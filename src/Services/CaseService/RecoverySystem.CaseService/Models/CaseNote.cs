using System;

namespace RecoverySystem.CaseService.Models;

public class CaseNote
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }

    // Foreign keys
    public Guid CaseId { get; set; }
    public Guid CreatedById { get; set; }

    // Navigation properties
    public virtual Case Case { get; set; }
}