using Microsoft.EntityFrameworkCore;
using RecoverySystem.NotificationService.Models;
using System.Text.Json;

namespace RecoverySystem.NotificationService.Data;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Notification> Notifications { get; set; }
    public DbSet<NotificationPreference> NotificationPreferences { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Notification entity
        modelBuilder.Entity<Notification>()
            .HasKey(n => n.Id);

        modelBuilder.Entity<Notification>()
            .Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(200);

        modelBuilder.Entity<Notification>()
            .Property(n => n.Message)
            .IsRequired()
            .HasMaxLength(1000);

        // Configure NotificationPreference entity
        modelBuilder.Entity<NotificationPreference>()
            .HasKey(p => p.Id);

        modelBuilder.Entity<NotificationPreference>()
            .Property(p => p.UserId)
            .IsRequired();

        // Store TypePreferences as JSON
        modelBuilder.Entity<NotificationPreference>()
            .Property(p => p.TypePreferences)
            .HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                v => JsonSerializer.Deserialize<List<NotificationTypePreference>>(v, new JsonSerializerOptions()));
    }
}