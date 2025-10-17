namespace Sub_Pal_API.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SubscriptionId { get; set; }
        public string SubscriptionName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public NotificationType NotificationType { get; set; } = NotificationType.PAYMENT_REMINDER;

        // Navigation properties
        public User User { get; set; } = null!;
        public Subscription Subscription { get; set; } = null!;
    }

    public enum NotificationType
    {
        PAYMENT_REMINDER,
        PAYMENT_DUE,
        PAYMENT_OVERDUE
    }
}
