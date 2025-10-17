namespace Sub_Pal_API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}
