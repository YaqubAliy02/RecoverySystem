namespace RecoverySystem.NotificationService.Models;

public class NotificationPreference
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public bool EmailEnabled { get; set; } = true;
    public bool InAppEnabled { get; set; } = true;
    public bool SmsEnabled { get; set; } = false;
    public List<NotificationTypePreference> TypePreferences { get; set; } = new List<NotificationTypePreference>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}