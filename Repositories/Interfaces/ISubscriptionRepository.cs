using Sub_Pal_API.Models;

namespace Sub_Pal_API.Repositories.Interfaces
{
    public interface ISubscriptionRepository
    {
        Task<List<Subscription>> GetAllByUserIdAsync(int userId);
        Task<List<Subscription>> GetAllAsync();
        Task<Subscription?> GetByIdAsync(int id, int userId);
        Task<Subscription> CreateAsync(Subscription subscription);
        Task<Subscription> UpdateAsync(Subscription subscription);
        Task DeleteAsync(Subscription subscription);
    }
}
