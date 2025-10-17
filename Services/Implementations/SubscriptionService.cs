using Sub_Pal_API.Models;
using Sub_Pal_API.Models.DTOs;
using Sub_Pal_API.Repositories.Interfaces;
using Sub_Pal_API.Services.Interfaces;

namespace Sub_Pal_API.Services.Implementations
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;

        public SubscriptionService(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<List<Subscription>> GetAllSubscriptionsAsync(int userId)
        {
            return await _subscriptionRepository.GetAllByUserIdAsync(userId);
        }

        public async Task<Subscription> CreateSubscriptionAsync(CreateSubscriptionDto dto, int userId)
        {
            var subscription = new Subscription
            {
                Name = dto.Name,
                Price = dto.Price,
                BillingCycle = dto.BillingCycle,
                NextRenewalDate = dto.NextRenewalDate,
                Category = dto.Category,
                UserId = userId
            };

            return await _subscriptionRepository.CreateAsync(subscription);
        }

        public async Task<bool> UpdateSubscriptionAsync(int id, UpdateSubscriptionDto dto, int userId)
        {
            var subscription = await _subscriptionRepository.GetByIdAsync(id, userId);
            if (subscription == null)
            {
                return false;
            }

            subscription.Name = dto.Name;
            subscription.Price = dto.Price;
            subscription.BillingCycle = dto.BillingCycle;
            subscription.NextRenewalDate = dto.NextRenewalDate;
            subscription.Category = dto.Category;

            await _subscriptionRepository.UpdateAsync(subscription);
            return true;
        }

        public async Task<bool> DeleteSubscriptionAsync(int id, int userId)
        {
            var subscription = await _subscriptionRepository.GetByIdAsync(id, userId);
            if (subscription == null)
            {
                return false;
            }

            await _subscriptionRepository.DeleteAsync(subscription);
            return true;
        }
    }
}
