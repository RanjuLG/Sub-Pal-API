namespace Sub_Pal_API.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        
        // Foreign key and navigation property
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
