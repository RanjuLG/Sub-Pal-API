using Sub_Pal_API.Models;
using Sub_Pal_API.Models.DTOs;
using Sub_Pal_API.Repositories.Interfaces;
using Sub_Pal_API.Services.Interfaces;

namespace Sub_Pal_API.Services.Implementations
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ICategoryService _categoryService;
        private readonly ICacheService _cacheService;

        public SubscriptionService(
            ISubscriptionRepository subscriptionRepository, 
            ICategoryService categoryService,
            ICacheService cacheService)
        {
            _subscriptionRepository = subscriptionRepository;
            _categoryService = categoryService;
            _cacheService = cacheService;
        }

        private string GetSubscriptionsCacheKey(int userId) => $"user:{userId}:subscriptions";
        private string GetUserCachePattern(int userId) => $"user:{userId}:*";

        public async Task<List<Subscription>> GetAllSubscriptionsAsync(int userId)
        {
            var cacheKey = GetSubscriptionsCacheKey(userId);
            
            // Try to get from cache
            var cachedSubscriptions = await _cacheService.GetAsync<List<Subscription>>(cacheKey);
            if (cachedSubscriptions != null)
            {
                return cachedSubscriptions;
            }

            // Get from database and cache
            var subscriptions = await _subscriptionRepository.GetAllByUserIdAsync(userId);
            await _cacheService.SetAsync(cacheKey, subscriptions, TimeSpan.FromMinutes(15));
            
            return subscriptions;
        }

        public async Task<Subscription> CreateSubscriptionAsync(CreateSubscriptionDto dto, int userId)
        {
            // Ensure category exists in the Categories table
            await _categoryService.EnsureCategoryExistsAsync(dto.Category, userId);

            var subscription = new Subscription
            {
                Name = dto.Name,
                Price = dto.Price,
                BillingCycle = dto.BillingCycle,
                NextRenewalDate = dto.NextRenewalDate,
                Category = dto.Category,
                UserId = userId,
                NotificationEnabled = dto.NotificationEnabled,
                NotificationDaysBefore = dto.NotificationDaysBefore,
                NotificationMessage = dto.NotificationMessage
            };

            var result = await _subscriptionRepository.CreateAsync(subscription);
            
            // Invalidate all user-related caches (subscriptions, dashboard)
            await _cacheService.RemoveByPatternAsync(GetUserCachePattern(userId));
            
            return result;
        }

        public async Task<bool> UpdateSubscriptionAsync(int id, UpdateSubscriptionDto dto, int userId)
        {
            var subscription = await _subscriptionRepository.GetByIdAsync(id, userId);
            if (subscription == null)
            {
                return false;
            }

            // Ensure category exists in the Categories table
            await _categoryService.EnsureCategoryExistsAsync(dto.Category, userId);

            subscription.Name = dto.Name;
            subscription.Price = dto.Price;
            subscription.BillingCycle = dto.BillingCycle;
            subscription.NextRenewalDate = dto.NextRenewalDate;
            subscription.Category = dto.Category;
            subscription.NotificationEnabled = dto.NotificationEnabled;
            subscription.NotificationDaysBefore = dto.NotificationDaysBefore;
            subscription.NotificationMessage = dto.NotificationMessage;

            await _subscriptionRepository.UpdateAsync(subscription);
            
            // Invalidate all user-related caches
            await _cacheService.RemoveByPatternAsync(GetUserCachePattern(userId));
            
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
            
            // Invalidate all user-related caches
            await _cacheService.RemoveByPatternAsync(GetUserCachePattern(userId));
            
            return true;
        }
    }
}
