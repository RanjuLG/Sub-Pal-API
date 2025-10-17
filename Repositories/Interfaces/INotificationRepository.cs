using Sub_Pal_API.Models;

namespace Sub_Pal_API.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetAllByUserIdAsync(int userId);
        Task<List<Notification>> GetUnreadByUserIdAsync(int userId);
        Task<Notification?> GetByIdAsync(int id, int userId);
        Task<Notification> CreateAsync(Notification notification);
        Task UpdateAsync(Notification notification);
        Task DeleteAsync(Notification notification);
        Task MarkAllAsReadAsync(int userId);
        Task<bool> NotificationExistsForSubscriptionAsync(int subscriptionId, DateTime dueDate);
    }
}
