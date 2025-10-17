namespace Sub_Pal_API.Models.DTOs
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public int SubscriptionId { get; set; }
        public string SubscriptionName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string NotificationType { get; set; } = string.Empty;
    }
}
