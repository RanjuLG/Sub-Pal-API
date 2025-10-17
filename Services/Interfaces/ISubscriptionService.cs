using Sub_Pal_API.Models;
using Sub_Pal_API.Models.DTOs;

namespace Sub_Pal_API.Services.Interfaces
{
    public interface ISubscriptionService
    {
        Task<List<Subscription>> GetAllSubscriptionsAsync(int userId);
        Task<Subscription> CreateSubscriptionAsync(CreateSubscriptionDto dto, int userId);
        Task<bool> UpdateSubscriptionAsync(int id, UpdateSubscriptionDto dto, int userId);
        Task<bool> DeleteSubscriptionAsync(int id, int userId);
    }
}
