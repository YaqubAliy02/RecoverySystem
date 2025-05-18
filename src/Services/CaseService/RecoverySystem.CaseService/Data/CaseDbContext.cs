using Microsoft.EntityFrameworkCore;
using RecoverySystem.CaseService.Models;

namespace RecoverySystem.CaseService.Data
{
    public class CaseDbContext : DbContext
    {
        public CaseDbContext(DbContextOptions<CaseDbContext> options) : base(options)
        {
        }

        public DbSet<Case> Cases { get; set; }
        public DbSet<CaseNote> CaseNotes { get; set; }
        public DbSet<CaseDocument> CaseDocuments { get; set; }
        public DbSet<CaseTimelineEvent> CaseTimelineEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Case configuration
            modelBuilder.Entity<Case>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(2000);
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.Priority).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                // Relationships
                entity.HasMany(e => e.Notes)
                      .WithOne(n => n.Case)
                      .HasForeignKey(n => n.CaseId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Documents)
                      .WithOne(d => d.Case)
                      .HasForeignKey(d => d.CaseId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // CaseNote configuration
            modelBuilder.Entity<CaseNote>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Content).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.CreatedAt).IsRequired();
            });

            // CaseDocument configuration
            modelBuilder.Entity<CaseDocument>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.ContentType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.FilePath).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.FileSize).IsRequired();
                entity.Property(e => e.UploadedAt).IsRequired();
            });
        }
    }
}