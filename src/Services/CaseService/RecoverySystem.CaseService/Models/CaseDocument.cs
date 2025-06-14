﻿using System;

namespace RecoverySystem.CaseService.Models;

public class CaseDocument
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public string FilePath { get; set; }
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; }
    public Guid CaseId { get; set; }
    public Guid UploadedById { get; set; }
    public virtual Case Case { get; set; }
}