using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sub_Pal_API.Models;
using Sub_Pal_API.Repositories.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sub_Pal_API.Services.Background
{
    public class SubscriptionBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SubscriptionBackgroundService> _logger;

        public SubscriptionBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<SubscriptionBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Subscription Background Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Subscription Background Service is working.");

                try
                {
                    await ProcessSubscriptionsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing subscriptions.");
                }

                // Run every 24 hours
                // For testing purposes, you might want to reduce this interval or make it configurable
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task ProcessSubscriptionsAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var subscriptionRepository = scope.ServiceProvider.GetRequiredService<ISubscriptionRepository>();
                var notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();

                var subscriptions = await subscriptionRepository.GetAllAsync();
                var today = DateTime.Now.Date;

                foreach (var sub in subscriptions)
                {
                    // 1. Check for Notifications
                    if (sub.NotificationEnabled)
                    {
                        var notificationDate = sub.NextRenewalDate.AddDays(-sub.NotificationDaysBefore).Date;

                        // If today is the notification date or past it (but before renewal), and we haven't notified yet
                        if (today >= notificationDate && today <= sub.NextRenewalDate.Date)
                        {
                            // Check if notification already exists for this cycle
                            bool exists = await notificationRepository.NotificationExistsForSubscriptionAsync(sub.Id, sub.NextRenewalDate);

                            if (!exists)
                            {
                                var notification = new Notification
                                {
                                    UserId = sub.UserId,
                                    SubscriptionId = sub.Id,
                                    SubscriptionName = sub.Name,
                                    Message = sub.NotificationMessage ?? $"Your subscription for {sub.Name} is due on {sub.NextRenewalDate:d}",
                                    Description = $"Amount: {sub.Price:C}",
                                    DueDate = sub.NextRenewalDate,
                                    IsRead = false,
                                    CreatedAt = DateTime.Now,
                                    NotificationType = NotificationType.PAYMENT_REMINDER
                                };

                                await notificationRepository.CreateAsync(notification);
                                _logger.LogInformation($"Created notification for subscription {sub.Id}");
                            }
                        }
                    }

                    // 2. Check for Auto-Renewal (Date Update)
                    if (today > sub.NextRenewalDate.Date)
                    {
                        // Update NextRenewalDate based on BillingCycle
                        // Assuming "Monthly" adds 1 month, "Annually" adds 1 year.
                        // Default to Monthly if unknown, or handle error? 
                        // Let's assume standard values.
                        
                        var oldDate = sub.NextRenewalDate;
                        DateTime newDate = oldDate;

                        if (string.Equals(sub.BillingCycle, "Monthly", StringComparison.OrdinalIgnoreCase))
                        {
                            newDate = oldDate.AddMonths(1);
                        }
                        else if (string.Equals(sub.BillingCycle, "Annually", StringComparison.OrdinalIgnoreCase) || 
                                 string.Equals(sub.BillingCycle, "Yearly", StringComparison.OrdinalIgnoreCase))
                        {
                            newDate = oldDate.AddYears(1);
                        }
                        else
                        {
                            // Fallback or log warning? Let's default to Monthly for safety or skip?
                            // If we skip, it will never update. Let's default to Monthly but log it.
                            _logger.LogWarning($"Unknown billing cycle '{sub.BillingCycle}' for subscription {sub.Id}. Defaulting to Monthly update.");
                            newDate = oldDate.AddMonths(1);
                        }

                        // While loop to catch up if multiple cycles missed?
                        // "Next Subscription date should be updated automatically"
                        // If the app was down for a year, we probably want to bring it to the *next* future date?
                        // Or just increment one step?
                        // Usually "Next Renewal" implies the upcoming one.
                        // If today is 2025, and renewal was 2023, we should probably update to 2025 or 2026.
                        
                        while (newDate < today)
                        {
                             if (string.Equals(sub.BillingCycle, "Monthly", StringComparison.OrdinalIgnoreCase))
                            {
                                newDate = newDate.AddMonths(1);
                            }
                            else if (string.Equals(sub.BillingCycle, "Annually", StringComparison.OrdinalIgnoreCase) || 
                                     string.Equals(sub.BillingCycle, "Yearly", StringComparison.OrdinalIgnoreCase))
                            {
                                newDate = newDate.AddYears(1);
                            }
                            else
                            {
                                newDate = newDate.AddMonths(1);
                            }
                        }

                        sub.NextRenewalDate = newDate;
                        await subscriptionRepository.UpdateAsync(sub);
                        _logger.LogInformation($"Updated subscription {sub.Id} renewal date from {oldDate:d} to {newDate:d}");
                    }
                }
            }
        }
    }
}
