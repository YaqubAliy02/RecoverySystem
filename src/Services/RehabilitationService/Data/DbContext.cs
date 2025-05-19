using Microsoft.EntityFrameworkCore;
using RecoverySystem.RehabilitationService.Models;
using System.Text.Json;

namespace RecoverySystem.RehabilitationService.Data
{
    public class RehabilitationDbContext : DbContext
    {
        public RehabilitationDbContext(DbContextOptions<RehabilitationDbContext> options)
            : base(options)
        {
        }

        public DbSet<RehabilitationProgram> RehabilitationPrograms { get; set; }
        public DbSet<RehabilitationActivity> RehabilitationActivities { get; set; }
        public DbSet<RehabilitationSession> RehabilitationSessions { get; set; }
        public DbSet<ProgressReport> ProgressReports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure RehabilitationProgram entity
            modelBuilder.Entity<RehabilitationProgram>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<RehabilitationProgram>()
                .Property(r => r.Title)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<RehabilitationProgram>()
                .Property(r => r.Description)
                .HasMaxLength(1000);

            modelBuilder.Entity<RehabilitationProgram>()
                .Property(r => r.PatientName)
                .HasMaxLength(200);

            modelBuilder.Entity<RehabilitationProgram>()
                .Property(r => r.AssignedToName)
                .HasMaxLength(200);

            modelBuilder.Entity<RehabilitationProgram>()
                .Property(r => r.Notes)
                .HasMaxLength(2000);

            // Configure RehabilitationActivity entity
            modelBuilder.Entity<RehabilitationActivity>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<RehabilitationActivity>()
                .Property(a => a.Title)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<RehabilitationActivity>()
                .Property(a => a.Description)
                .HasMaxLength(1000);

            modelBuilder.Entity<RehabilitationActivity>()
                .Property(a => a.Instructions)
                .HasMaxLength(2000);

            modelBuilder.Entity<RehabilitationActivity>()
                .Property(a => a.VideoUrl)
                .HasMaxLength(500);

            modelBuilder.Entity<RehabilitationActivity>()
                .Property(a => a.ImageUrl)
                .HasMaxLength(500);

            // Configure RehabilitationSession entity
            modelBuilder.Entity<RehabilitationSession>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<RehabilitationSession>()
                .Property(s => s.Notes)
                .HasMaxLength(2000);

            modelBuilder.Entity<RehabilitationSession>()
                .Property(s => s.SupervisedByName)
                .HasMaxLength(200);

            // Store CompletedActivities as JSON
            modelBuilder.Entity<RehabilitationSession>()
                .Property(s => s.CompletedActivities)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<List<ActivityCompletion>>(v, new JsonSerializerOptions()));

            // Configure ProgressReport entity
            modelBuilder.Entity<ProgressReport>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<ProgressReport>()
                .Property(p => p.PatientName)
                .HasMaxLength(200);

            modelBuilder.Entity<ProgressReport>()
                .Property(p => p.Assessment)
                .HasMaxLength(2000);

            modelBuilder.Entity<ProgressReport>()
                .Property(p => p.Recommendations)
                .HasMaxLength(2000);

            modelBuilder.Entity<ProgressReport>()
                .Property(p => p.CreatedByName)
                .HasMaxLength(200);

            // Store ActivityProgress as JSON
            modelBuilder.Entity<ProgressReport>()
                .Property(p => p.ActivityProgress)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<List<ActivityProgress>>(v, new JsonSerializerOptions()));
        }
    }
}