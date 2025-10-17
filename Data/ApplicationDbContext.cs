using Microsoft.EntityFrameworkCore;
using Sub_Pal_API.Models;

namespace Sub_Pal_API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.PasswordSalt).IsRequired();
            });

            // Configure Subscription entity
            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Name).IsRequired().HasMaxLength(100);
                entity.Property(s => s.Price).HasColumnType("decimal(18,2)");
                entity.Property(s => s.BillingCycle).IsRequired().HasMaxLength(20);
                entity.Property(s => s.Category).IsRequired().HasMaxLength(50);

                // Configure one-to-many relationship
                entity.HasOne(s => s.User)
                      .WithMany(u => u.Subscriptions)
                      .HasForeignKey(s => s.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Category entity
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(50);

                // Configure one-to-many relationship
                entity.HasOne(c => c.User)
                      .WithMany(u => u.Categories)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Ensure category names are unique per user
                entity.HasIndex(c => new { c.UserId, c.Name }).IsUnique();
            });

            // Configure Notification entity
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(n => n.Id);
                entity.Property(n => n.SubscriptionName).IsRequired().HasMaxLength(255);
                entity.Property(n => n.Message).IsRequired().HasMaxLength(500);
                entity.Property(n => n.Description).HasMaxLength(1000);
                entity.Property(n => n.NotificationType)
                      .HasConversion<string>();

                // Configure relationships
                entity.HasOne(n => n.User)
                      .WithMany()
                      .HasForeignKey(n => n.UserId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(n => n.Subscription)
                      .WithMany()
                      .HasForeignKey(n => n.SubscriptionId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Create indexes for better query performance
                entity.HasIndex(n => n.UserId);
                entity.HasIndex(n => n.IsRead);
                entity.HasIndex(n => n.DueDate);
            });
        }
    }
}
