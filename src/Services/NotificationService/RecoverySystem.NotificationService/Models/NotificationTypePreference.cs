using RecoverySystem.NotificationService.Models.Enum;

namespace RecoverySystem.NotificationService.Models;

public class NotificationTypePreference
{
    public NotificationType Type { get; set; }
    public bool EmailEnabled { get; set; } = true;
    public bool InAppEnabled { get; set; } = true;
    public bool SmsEnabled { get; set; } = false;
}
