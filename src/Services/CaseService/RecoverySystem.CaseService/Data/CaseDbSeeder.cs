// Data/CaseDbSeeder.cs
using RecoverySystem.CaseService.Models;

namespace RecoverySystem.CaseService.Data
{
    public static class CaseDbSeeder
    {
        public static async Task SeedAsync(CaseDbContext context)
        {
            // Only seed if the database is empty
            if (context.Cases.Any())
            {
                return;
            }

            // Create sample cases
            var cases = new List<Case>
            {
                new Case
                {
                    Id = Guid.NewGuid().ToString(),
                    CaseNumber = "CS-20230501-0001",
                    Title = "Initial Assessment",
                    Description = "Initial assessment for patient after surgery",
                    PatientId = "patient-001",
                    PatientName = "John Doe",
                    Status = "open",
                    Priority = "high",
                    AssignedToId = "user-001",
                    AssignedToName = "Dr. Smith",
                    AssignedToAvatar = "/placeholder.svg?height=40&width=40",
                    Category = "assessment",
                    Type = "post-surgery",
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    UpdatedAt = DateTime.UtcNow.AddDays(-5),
                    DueDate = DateTime.UtcNow.AddDays(5)
                },
                new Case
                {
                    Id = Guid.NewGuid().ToString(),
                    CaseNumber = "CS-20230502-0002",
                    Title = "Physical Therapy Plan",
                    Description = "Create a physical therapy plan for knee rehabilitation",
                    PatientId = "patient-002",
                    PatientName = "Jane Smith",
                    Status = "in-progress",
                    Priority = "medium",
                    AssignedToId = "user-002",
                    AssignedToName = "Dr. Johnson",
                    AssignedToAvatar = "/placeholder.svg?height=40&width=40",
                    Category = "therapy",
                    Type = "physical",
                    CreatedAt = DateTime.UtcNow.AddDays(-7),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2),
                    DueDate = DateTime.UtcNow.AddDays(10)
                },
                new Case
                {
                    Id = Guid.NewGuid().ToString(),
                    CaseNumber = "CS-20230503-0003",
                    Title = "Follow-up Appointment",
                    Description = "Schedule follow-up appointment for medication review",
                    PatientId = "patient-003",
                    PatientName = "Robert Brown",
                    Status = "closed",
                    Priority = "low",
                    AssignedToId = "user-003",
                    AssignedToName = "Dr. Williams",
                    AssignedToAvatar = "/placeholder.svg?height=40&width=40",
                    Category = "follow-up",
                    Type = "medication",
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1),
                    ClosedAt = DateTime.UtcNow.AddDays(-1),
                    DueDate = DateTime.UtcNow.AddDays(-2)
                }
            };

            await context.Cases.AddRangeAsync(cases);

            // Add notes to the first case
            var notes = new List<CaseNote>
            {
                new CaseNote
                {
                    Id = Guid.NewGuid().ToString(),
                    CaseId = cases[0].Id,
                    Content = "Patient reports pain level of 7/10 in the affected area.",
                    CreatedAt = DateTime.UtcNow.AddDays(-9),
                    AuthorId = "user-001",
                    AuthorName = "Dr. Smith",
                    AuthorAvatar = "/placeholder.svg?height=40&width=40",
                    AuthorRole = "doctor"
                },
                new CaseNote
                {
                    Id = Guid.NewGuid().ToString(),
                    CaseId = cases[0].Id,
                    Content = "Prescribed pain medication and recommended rest for 48 hours.",
                    CreatedAt = DateTime.UtcNow.AddDays(-8),
                    AuthorId = "user-001",
                    AuthorName = "Dr. Smith",
                    AuthorAvatar = "/placeholder.svg?height=40&width=40",
                    AuthorRole = "doctor"
                }
            };

            await context.CaseNotes.AddRangeAsync(notes);

            // Add documents to the first case
            var documents = new List<CaseDocument>
            {
                new CaseDocument
                {
                    Id = Guid.NewGuid().ToString(),
                    CaseId = cases[0].Id,
                    FileName = "xray-results.pdf",
                    FileType = "application/pdf",
                    FileSize = "2.5 MB",
                    FileUrl = "/api/documents/xray-results.pdf",
                    UploadedAt = DateTime.UtcNow.AddDays(-9),
                    UploadedById = "user-001",
                    UploadedByName = "Dr. Smith",
                    UploadedByAvatar = "/placeholder.svg?height=40&width=40",
                    Description = "X-ray results from initial assessment"
                },
                new CaseDocument
                {
                    Id = Guid.NewGuid().ToString(),
                    CaseId = cases[0].Id,
                    FileName = "treatment-plan.docx",
                    FileType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    FileSize = "1.2 MB",
                    FileUrl = "/api/documents/treatment-plan.docx",
                    UploadedAt = DateTime.UtcNow.AddDays(-8),
                    UploadedById = "user-001",
                    UploadedByName = "Dr. Smith",
                    UploadedByAvatar = "/placeholder.svg?height=40&width=40",
                    Description = "Initial treatment plan"
                }
            };

            await context.CaseDocuments.AddRangeAsync(documents);

            // Add timeline events to the first case
            var timelineEvents = new List<CaseTimelineEvent>
            {
                new CaseTimelineEvent
                {
                    Id = Guid.NewGuid().ToString(),
                    CaseId = cases[0].Id,
                    EventType = "created",
                    Description = "Case was created",
                    Timestamp = DateTime.UtcNow.AddDays(-10),
                    UserId = "user-001",
                    UserName = "Dr. Smith",
                    UserAvatar = "/placeholder.svg?height=40&width=40"
                },
                new CaseTimelineEvent
                {
                    Id = Guid.NewGuid().ToString(),
                    CaseId = cases[0].Id,
                    EventType = "note_added",
                    Description = "Note was added to the case",
                    Timestamp = DateTime.UtcNow.AddDays(-9),
                    UserId = "user-001",
                    UserName = "Dr. Smith",
                    UserAvatar = "/placeholder.svg?height=40&width=40"
                },
                new CaseTimelineEvent
                {
                    Id = Guid.NewGuid().ToString(),
                    CaseId = cases[0].Id,
                    EventType = "document_uploaded",
                    Description = "Document 'xray-results.pdf' was uploaded",
                    Timestamp = DateTime.UtcNow.AddDays(-9),
                    UserId = "user-001",
                    UserName = "Dr. Smith",
                    UserAvatar = "/placeholder.svg?height=40&width=40",
                    Metadata = "{\"fileName\":\"xray-results.pdf\",\"fileSize\":\"2.5 MB\"}"
                },
                new CaseTimelineEvent
                {
                    Id = Guid.NewGuid().ToString(),
                    CaseId = cases[0].Id,
                    EventType = "note_added",
                    Description = "Note was added to the case",
                    Timestamp = DateTime.UtcNow.AddDays(-8),
                    UserId = "user-001",
                    UserName = "Dr. Smith",
                    UserAvatar = "/placeholder.svg?height=40&width=40"
                },
                new CaseTimelineEvent
                {
                    Id = Guid.NewGuid().ToString(),
                    CaseId = cases[0].Id,
                    EventType = "document_uploaded",
                    Description = "Document 'treatment-plan.docx' was uploaded",
                    Timestamp = DateTime.UtcNow.AddDays(-8),
                    UserId = "user-001",
                    UserName = "Dr. Smith",
                    UserAvatar = "/placeholder.svg?height=40&width=40",
                    Metadata = "{\"fileName\":\"treatment-plan.docx\",\"fileSize\":\"1.2 MB\"}"
                },
                new CaseTimelineEvent
                {
                    Id = Guid.NewGuid().ToString(),
                    CaseId = cases[0].Id,
                    EventType = "status_changed",
                    Description = "Status changed from 'new' to 'open'",
                    Timestamp = DateTime.UtcNow.AddDays(-5),
                    UserId = "user-001",
                    UserName = "Dr. Smith",
                    UserAvatar = "/placeholder.svg?height=40&width=40",
                    Metadata = "{\"oldStatus\":\"new\",\"newStatus\":\"open\"}"
                }
            };

            await context.CaseTimelineEvents.AddRangeAsync(timelineEvents);

            await context.SaveChangesAsync();
        }
    }
}