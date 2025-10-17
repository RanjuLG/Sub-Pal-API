namespace Sub_Pal_API.Models.DTOs
{
    public class CreateNotificationDto
    {
        public int SubscriptionId { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
    }
}
