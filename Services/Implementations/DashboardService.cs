using Sub_Pal_API.Models.DTOs;
using Sub_Pal_API.Repositories.Interfaces;
using Sub_Pal_API.Services.Interfaces;

namespace Sub_Pal_API.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;

        public DashboardService(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(int userId)
        {
            var subscriptions = await _subscriptionRepository.GetAllByUserIdAsync(userId);

            // Calculate total monthly cost
            decimal totalMonthlyCost = 0;
            foreach (var sub in subscriptions)
            {
                if (sub.BillingCycle.Equals("Monthly", StringComparison.OrdinalIgnoreCase))
                {
                    totalMonthlyCost += sub.Price;
                }
                else if (sub.BillingCycle.Equals("Annually", StringComparison.OrdinalIgnoreCase))
                {
                    totalMonthlyCost += sub.Price / 12;
                }
            }

            // Calculate total annual cost
            decimal totalAnnualCost = 0;
            foreach (var sub in subscriptions)
            {
                if (sub.BillingCycle.Equals("Monthly", StringComparison.OrdinalIgnoreCase))
                {
                    totalAnnualCost += sub.Price * 12;
                }
                else if (sub.BillingCycle.Equals("Annually", StringComparison.OrdinalIgnoreCase))
                {
                    totalAnnualCost += sub.Price;
                }
            }

            // Calculate spending by category
            var spendingByCategory = subscriptions
                .GroupBy(s => s.Category)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(s => s.BillingCycle.Equals("Monthly", StringComparison.OrdinalIgnoreCase) 
                        ? s.Price * 12 
                        : s.Price)
                );

            // Get upcoming renewals (next 30 days)
            var upcomingRenewals = subscriptions
                .Where(s => s.NextRenewalDate <= DateTime.UtcNow.AddDays(30) && s.NextRenewalDate >= DateTime.UtcNow)
                .OrderBy(s => s.NextRenewalDate)
                .ToList();

            return new DashboardSummaryDto
            {
                TotalMonthlyCost = totalMonthlyCost,
                TotalAnnualCost = totalAnnualCost,
                SpendingByCategory = spendingByCategory,
                UpcomingRenewals = upcomingRenewals
            };
        }
    }
}
