using Sub_Pal_API.Models;
using Sub_Pal_API.Models.DTOs;
using Sub_Pal_API.Repositories.Interfaces;
using Sub_Pal_API.Services.Interfaces;

namespace Sub_Pal_API.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ICacheService _cacheService;

        public NotificationService(
            INotificationRepository notificationRepository,
            ISubscriptionRepository subscriptionRepository,
            ICacheService cacheService)
        {
            _notificationRepository = notificationRepository;
            _subscriptionRepository = subscriptionRepository;
            _cacheService = cacheService;
        }

        private string GetNotificationsCacheKey(int userId) => $"user:{userId}:notifications";
        private string GetUnreadNotificationsCacheKey(int userId) => $"user:{userId}:notifications:unread";

        public async Task<List<NotificationDto>> GetUserNotificationsAsync(int userId)
        {
            var cacheKey = GetNotificationsCacheKey(userId);
            
            // Try to get from cache
            var cachedNotifications = await _cacheService.GetAsync<List<NotificationDto>>(cacheKey);
            if (cachedNotifications != null)
            {
                return cachedNotifications;
            }

            // Get from database and cache
            var notifications = await _notificationRepository.GetAllByUserIdAsync(userId);
            var dtos = notifications.Select(MapToDto).ToList();
            await _cacheService.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(10));
            
            return dtos;
        }

        public async Task<List<NotificationDto>> GetUnreadNotificationsAsync(int userId)
        {
            var cacheKey = GetUnreadNotificationsCacheKey(userId);
            
            // Try to get from cache
            var cachedNotifications = await _cacheService.GetAsync<List<NotificationDto>>(cacheKey);
            if (cachedNotifications != null)
            {
                return cachedNotifications;
            }

            // Get from database and cache
            var notifications = await _notificationRepository.GetUnreadByUserIdAsync(userId);
            var dtos = notifications.Select(MapToDto).ToList();
            await _cacheService.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(5));
            
            return dtos;
        }

        public async Task MarkAsReadAsync(int notificationId, int userId)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId, userId);
            if (notification == null)
            {
                throw new Exception("Notification not found");
            }

            notification.IsRead = true;
            await _notificationRepository.UpdateAsync(notification);
            
            // Invalidate notifications cache
            await _cacheService.RemoveAsync(GetNotificationsCacheKey(userId));
            await _cacheService.RemoveAsync(GetUnreadNotificationsCacheKey(userId));
        }

        public async Task MarkAllAsReadAsync(int userId)
        {
            await _notificationRepository.MarkAllAsReadAsync(userId);
            
            // Invalidate notifications cache
            await _cacheService.RemoveAsync(GetNotificationsCacheKey(userId));
            await _cacheService.RemoveAsync(GetUnreadNotificationsCacheKey(userId));
        }

        public async Task DeleteNotificationAsync(int notificationId, int userId)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId, userId);
            if (notification == null)
            {
                throw new Exception("Notification not found");
            }

            await _notificationRepository.DeleteAsync(notification);
            
            // Invalidate notifications cache
            await _cacheService.RemoveAsync(GetNotificationsCacheKey(userId));
            await _cacheService.RemoveAsync(GetUnreadNotificationsCacheKey(userId));
        }

        public async Task GenerateNotificationsAsync()
        {
            // This method would be called by a background job
            // For now, it's a placeholder for the scheduled task
            var today = DateTime.Now.Date;

            // Get all subscriptions with notifications enabled
            // Note: This would need a new repository method to get all subscriptions across all users
            // For now, leaving this as a placeholder
            
            // Implementation would:
            // 1. Get all subscriptions with NotificationEnabled = true
            // 2. Check if notification date matches today
            // 3. Create notifications if they don't already exist
        }

        private NotificationDto MapToDto(Notification notification)
        {
            return new NotificationDto
            {
                Id = notification.Id,
                SubscriptionId = notification.SubscriptionId,
                SubscriptionName = notification.SubscriptionName,
                Message = notification.Message,
                Description = notification.Description,
                DueDate = notification.DueDate,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt,
                NotificationType = notification.NotificationType.ToString()
            };
        }
    }
}
