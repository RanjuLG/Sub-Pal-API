using Sub_Pal_API.Models;
using Sub_Pal_API.Models.DTOs;

namespace Sub_Pal_API.Services.Interfaces
{
    public interface INotificationService
    {
        Task<List<NotificationDto>> GetUserNotificationsAsync(int userId);
        Task<List<NotificationDto>> GetUnreadNotificationsAsync(int userId);
        Task MarkAsReadAsync(int notificationId, int userId);
        Task MarkAsUnreadAsync(int notificationId, int userId);
        Task MarkAllAsReadAsync(int userId);
        Task DeleteNotificationAsync(int notificationId, int userId);
        Task GenerateNotificationsAsync();
    }
}
