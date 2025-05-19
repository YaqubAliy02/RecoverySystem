using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecoverySystem.NotificationService.Data;
using RecoverySystem.NotificationService.DTOs;
using RecoverySystem.NotificationService.Models;
using RecoverySystem.NotificationService.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecoverySystem.NotificationService.Services
{
    public class NotificationService : INotificationService
    {
        private readonly NotificationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<NotificationService> _logger;
        private readonly EventPublisher _eventPublisher;

        public NotificationService(
            NotificationDbContext dbContext,
            IMapper mapper,
            ILogger<NotificationService> logger,
            EventPublisher eventPublisher)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public async Task<IEnumerable<NotificationDto>> GetNotificationsForUserAsync(string userId, bool includeRead = false)
        {
            var query = _dbContext.Notifications
                .Where(n => n.TargetUserId == userId);

            if (!includeRead)
            {
                query = query.Where(n => !n.IsRead);
            }

            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
        }

        public async Task<NotificationDto> GetNotificationByIdAsync(Guid id)
        {
            var notification = await _dbContext.Notifications.FindAsync(id);
            if (notification == null)
            {
                _logger.LogWarning("Notification with ID {NotificationId} not found", id);
                return null;
            }

            return _mapper.Map<NotificationDto>(notification);
        }

        public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto createDto)
        {
            var notification = _mapper.Map<Notification>(createDto);
            notification.Id = Guid.NewGuid();
            notification.IsRead = false;
            notification.CreatedAt = DateTime.UtcNow;

            await _dbContext.Notifications.AddAsync(notification);
            await _dbContext.SaveChangesAsync();

            // Publish event
            try
            {
                await _eventPublisher.PublishNotificationCreatedEventAsync(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish NotificationCreatedEvent for notification {NotificationId}", notification.Id);
            }

            return _mapper.Map<NotificationDto>(notification);
        }

        public async Task<NotificationDto> MarkNotificationAsReadAsync(Guid id, bool isRead)
        {
            var notification = await _dbContext.Notifications.FindAsync(id);
            if (notification == null)
            {
                _logger.LogWarning("Notification with ID {NotificationId} not found", id);
                return null;
            }

            notification.IsRead = isRead;
            if (isRead && !notification.ReadAt.HasValue)
            {
                notification.ReadAt = DateTime.UtcNow;
            }
            else if (!isRead)
            {
                notification.ReadAt = null;
            }

            _dbContext.Notifications.Update(notification);
            await _dbContext.SaveChangesAsync();

            // Publish event if marked as read
            if (isRead)
            {
                try
                {
                    await _eventPublisher.PublishNotificationReadEventAsync(notification);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to publish NotificationReadEvent for notification {NotificationId}", notification.Id);
                }
            }

            return _mapper.Map<NotificationDto>(notification);
        }

        public async Task<int> MarkAllNotificationsAsReadAsync(string userId)
        {
            var notifications = await _dbContext.Notifications
                .Where(n => n.TargetUserId == userId && !n.IsRead)
                .ToListAsync();

            if (!notifications.Any())
            {
                return 0;
            }

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
            }

            _dbContext.Notifications.UpdateRange(notifications);
            await _dbContext.SaveChangesAsync();

            // Publish events
            try
            {
                foreach (var notification in notifications)
                {
                    await _eventPublisher.PublishNotificationReadEventAsync(notification);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish NotificationReadEvents for user {UserId}", userId);
            }

            return notifications.Count;
        }

        public async Task<NotificationPreferenceDto> GetUserPreferencesAsync(string userId)
        {
            var preferences = await _dbContext.NotificationPreferences
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (preferences == null)
            {
                // Create default preferences if none exist
                preferences = new NotificationPreference
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    EmailEnabled = true,
                    InAppEnabled = true,
                    SmsEnabled = false,
                    TypePreferences = Enum.GetValues(typeof(NotificationType))
                        .Cast<NotificationType>()
                        .Select(t => new NotificationTypePreference
                        {
                            Type = t,
                            EmailEnabled = true,
                            InAppEnabled = true,
                            SmsEnabled = false
                        })
                        .ToList(),
                    CreatedAt = DateTime.UtcNow
                };

                await _dbContext.NotificationPreferences.AddAsync(preferences);
                await _dbContext.SaveChangesAsync();
            }

            return _mapper.Map<NotificationPreferenceDto>(preferences);
        }

        public async Task<NotificationPreferenceDto> UpdateUserPreferencesAsync(string userId, UpdateNotificationPreferenceDto updateDto)
        {
            var preferences = await _dbContext.NotificationPreferences
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (preferences == null)
            {
                preferences = new NotificationPreference
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                await _dbContext.NotificationPreferences.AddAsync(preferences);
            }

            preferences.EmailEnabled = updateDto.EmailEnabled;
            preferences.InAppEnabled = updateDto.InAppEnabled;
            preferences.SmsEnabled = updateDto.SmsEnabled;
            preferences.TypePreferences = updateDto.TypePreferences
                .Select(tp => _mapper.Map<NotificationTypePreference>(tp))
                .ToList();
            preferences.UpdatedAt = DateTime.UtcNow;

            if (preferences.Id == Guid.Empty)
            {
                await _dbContext.NotificationPreferences.AddAsync(preferences);
            }
            else
            {
                _dbContext.NotificationPreferences.Update(preferences);
            }

            await _dbContext.SaveChangesAsync();

            return _mapper.Map<NotificationPreferenceDto>(preferences);
        }

        public async Task<bool> ShouldSendNotificationAsync(string userId, NotificationType type, string channel)
        {
            var preferences = await _dbContext.NotificationPreferences
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (preferences == null)
            {
                // Default to true if no preferences are set
                return true;
            }

            // Check global settings first
            bool globalSetting = channel switch
            {
                "email" => preferences.EmailEnabled,
                "inapp" => preferences.InAppEnabled,
                "sms" => preferences.SmsEnabled,
                _ => true
            };

            if (!globalSetting)
            {
                return false;
            }

            // Check type-specific settings
            var typePreference = preferences.TypePreferences
                .FirstOrDefault(tp => tp.Type == type);

            if (typePreference == null)
            {
                return true;
            }

            return channel switch
            {
                "email" => typePreference.EmailEnabled,
                "inapp" => typePreference.InAppEnabled,
                "sms" => typePreference.SmsEnabled,
                _ => true
            };
        }
    }
}