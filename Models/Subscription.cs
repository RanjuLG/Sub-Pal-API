namespace Sub_Pal_API.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string BillingCycle { get; set; } = string.Empty; // "Monthly" or "Annually"
        public DateTime NextRenewalDate { get; set; }
        public string Category { get; set; } = string.Empty;

        // Notification settings
        public bool NotificationEnabled { get; set; } = false;
        public int NotificationDaysBefore { get; set; } = 3;
        public string? NotificationMessage { get; set; }

        // Foreign key and navigation property
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
