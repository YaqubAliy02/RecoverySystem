using RecoverySystem.NotificationService.Models.Enum;

namespace RecoverySystem.NotificationService.DTOs;

public class NotificationDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public NotificationType Type { get; set; }
    public string TargetUserId { get; set; }
    public string SourceId { get; set; }
    public string SourceType { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }
}

public class CreateNotificationDto
{
    public string Title { get; set; }
    public string Message { get; set; }
    public NotificationType Type { get; set; }
    public string TargetUserId { get; set; }
    public string SourceId { get; set; }
    public string SourceType { get; set; }
}

public class MarkNotificationReadDto
{
    public bool IsRead { get; set; }
}