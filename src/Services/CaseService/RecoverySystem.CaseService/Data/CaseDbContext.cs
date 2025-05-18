// Data/CaseDbContext.cs
using Microsoft.EntityFrameworkCore;
using RecoverySystem.CaseService.Models;

namespace RecoverySystem.CaseService.Data;

public class CaseDbContext : DbContext
{
    public CaseDbContext(DbContextOptions<CaseDbContext> options)
        : base(options)
    {
    }

    public DbSet<Case> Cases { get; set; }
    public DbSet<CaseNote> CaseNotes { get; set; }
    public DbSet<CaseDocument> CaseDocuments { get; set; }
    public DbSet<CaseTimelineEvent> CaseTimelineEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Case entity
        modelBuilder.Entity<Case>()
            .HasKey(c => c.Id);

        modelBuilder.Entity<Case>()
            .Property(c => c.Title)
            .IsRequired();

        modelBuilder.Entity<Case>()
            .Property(c => c.PatientId)
            .IsRequired();

        modelBuilder.Entity<Case>()
            .HasMany(c => c.Notes)
            .WithOne(n => n.Case)
            .HasForeignKey(n => n.CaseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Case>()
            .HasMany(c => c.Documents)
            .WithOne(d => d.Case)
            .HasForeignKey(d => d.CaseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Case>()
            .HasMany(c => c.TimelineEvents)
            .WithOne(t => t.Case)
            .HasForeignKey(t => t.CaseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure CaseNote entity
        modelBuilder.Entity<CaseNote>()
            .HasKey(n => n.Id);

        modelBuilder.Entity<CaseNote>()
            .Property(n => n.Content)
            .IsRequired();

        // Configure CaseDocument entity
        modelBuilder.Entity<CaseDocument>()
            .HasKey(d => d.Id);

        modelBuilder.Entity<CaseDocument>()
            .Property(d => d.FileName)
            .IsRequired();

        // Configure CaseTimelineEvent entity
        modelBuilder.Entity<CaseTimelineEvent>()
            .HasKey(t => t.Id);

        modelBuilder.Entity<CaseTimelineEvent>()
            .Property(t => t.EventType)
            .IsRequired();
    }
}