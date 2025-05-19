using RecoverySystem.NotificationService.Models.Enum;

namespace RecoverySystem.NotificationService.Models;

public class Notification
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public NotificationType Type { get; set; }
    public string TargetUserId { get; set; }
    public string SourceId { get; set; }
    public string SourceType { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReadAt { get; set; }
}
