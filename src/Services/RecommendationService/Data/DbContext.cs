using Microsoft.EntityFrameworkCore;
using RecoverySystem.RecommendationService.Models;
using System.Text.Json;

namespace RecoverySystem.RecommendationService.Data
{
    public class RecommendationDbContext : DbContext
    {
        public RecommendationDbContext(DbContextOptions<RecommendationDbContext> options)
        : base(options)
        {
        }

        public DbSet<Recommendation> Recommendations { get; set; }
        public DbSet<RecommendationFeedback> RecommendationFeedbacks { get; set; }
        public DbSet<RecommendationTemplate> RecommendationTemplates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Recommendation entity
            modelBuilder.Entity<Recommendation>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<Recommendation>()
                .Property(r => r.Title)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<Recommendation>()
                .Property(r => r.Description)
                .HasMaxLength(1000);

            modelBuilder.Entity<Recommendation>()
                .Property(r => r.PatientName)
                .HasMaxLength(200);

            modelBuilder.Entity<Recommendation>()
                .Property(r => r.Instructions)
                .HasMaxLength(2000);

            modelBuilder.Entity<Recommendation>()
                .Property(r => r.CreatedByName)
                .HasMaxLength(200);

            modelBuilder.Entity<Recommendation>()
                .Property(r => r.ApprovedByName)
                .HasMaxLength(200);

            modelBuilder.Entity<Recommendation>()
                .Property(r => r.Notes)
                .HasMaxLength(2000);

            // Store Tags as JSON
            modelBuilder.Entity<Recommendation>()
                .Property(r => r.Tags)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<List<string>>(v, new JsonSerializerOptions()));

            // Configure RecommendationFeedback entity
            modelBuilder.Entity<RecommendationFeedback>()
                .HasKey(f => f.Id);

            modelBuilder.Entity<RecommendationFeedback>()
                .Property(f => f.Comment)
                .HasMaxLength(1000);

            modelBuilder.Entity<RecommendationFeedback>()
                .Property(f => f.Challenges)
                .HasMaxLength(1000);

            modelBuilder.Entity<RecommendationFeedback>()
                .Property(f => f.Improvements)
                .HasMaxLength(1000);

            modelBuilder.Entity<RecommendationFeedback>()
                .Property(f => f.ProvidedByName)
                .HasMaxLength(200);

            // Configure RecommendationTemplate entity
            modelBuilder.Entity<RecommendationTemplate>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<RecommendationTemplate>()
                .Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<RecommendationTemplate>()
                .Property(t => t.Description)
                .HasMaxLength(1000);

            modelBuilder.Entity<RecommendationTemplate>()
                .Property(t => t.Instructions)
                .HasMaxLength(2000);

            modelBuilder.Entity<RecommendationTemplate>()
                .Property(t => t.CreatedByName)
                .HasMaxLength(200);

            // Store Tags as JSON
            modelBuilder.Entity<RecommendationTemplate>()
                .Property(t => t.Tags)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<List<string>>(v, new JsonSerializerOptions()));
        }
    }
}