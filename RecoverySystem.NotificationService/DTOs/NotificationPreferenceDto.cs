using RecoverySystem.NotificationService.Models.Enum;

namespace RecoverySystem.NotificationService.DTOs;

public class NotificationPreferenceDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public bool EmailEnabled { get; set; }
    public bool InAppEnabled { get; set; }
    public bool SmsEnabled { get; set; }
    public List<NotificationTypePreferenceDto> TypePreferences { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class NotificationTypePreferenceDto
{
    public NotificationType Type { get; set; }
    public bool EmailEnabled { get; set; }
    public bool InAppEnabled { get; set; }
    public bool SmsEnabled { get; set; }
}

public class UpdateNotificationPreferenceDto
{
    public bool EmailEnabled { get; set; }
    public bool InAppEnabled { get; set; }
    public bool SmsEnabled { get; set; }
    public List<NotificationTypePreferenceDto> TypePreferences { get; set; }
}