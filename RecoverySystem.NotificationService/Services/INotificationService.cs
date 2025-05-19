using RecoverySystem.NotificationService.DTOs;
using RecoverySystem.NotificationService.Models.Enum;

namespace RecoverySystem.NotificationService.Services;

public interface INotificationService
{
    Task<IEnumerable<NotificationDto>> GetNotificationsForUserAsync(string userId, bool includeRead = false);
    Task<NotificationDto> GetNotificationByIdAsync(Guid id);
    Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto createDto);
    Task<NotificationDto> MarkNotificationAsReadAsync(Guid id, bool isRead);
    Task<int> MarkAllNotificationsAsReadAsync(string userId);
    Task<NotificationPreferenceDto> GetUserPreferencesAsync(string userId);
    Task<NotificationPreferenceDto> UpdateUserPreferencesAsync(string userId, UpdateNotificationPreferenceDto updateDto);
    Task<bool> ShouldSendNotificationAsync(string userId, NotificationType type, string channel);
}