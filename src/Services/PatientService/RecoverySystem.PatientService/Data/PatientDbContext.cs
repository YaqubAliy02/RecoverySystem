using Microsoft.EntityFrameworkCore;
using RecoverySystem.PatientService.Models;

namespace RecoverySystem.PatientService.Data;

public class PatientDbContext : DbContext
{
    public PatientDbContext(DbContextOptions<PatientDbContext> options)
        : base(options)
    {
    }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<PatientVital> PatientVitals { get; set; }
    public DbSet<PatientNote> PatientNotes { get; set; }
    public DbSet<PatientRecommendation> PatientRecommendations { get; set; }
    public DbSet<PatientRehabilitation> PatientRehabilitations { get; set; }
    public DbSet<RehabilitationExercise> RehabilitationExercises { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Patient entity
        modelBuilder.Entity<Patient>()
            .HasKey(p => p.Id);

        modelBuilder.Entity<Patient>()
            .Property(p => p.Name)
            .IsRequired();

        modelBuilder.Entity<Patient>()
            .HasMany(p => p.Vitals)
            .WithOne(v => v.Patient)
            .HasForeignKey(v => v.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Patient>()
            .HasMany(p => p.Notes)
            .WithOne(n => n.Patient)
            .HasForeignKey(n => n.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        // Convert List<string> to JSON
        modelBuilder.Entity<Patient>()
            .Property(p => p.Medications)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

        // Configure PatientVital entity
        modelBuilder.Entity<PatientVital>()
            .HasKey(v => v.Id);

        // Configure PatientNote entity
        modelBuilder.Entity<PatientNote>()
            .HasKey(n => n.Id);

        // Configure PatientRecommendation entity
        modelBuilder.Entity<PatientRecommendation>()
            .HasKey(r => r.Id);

        modelBuilder.Entity<PatientRecommendation>()
            .HasOne(r => r.Patient)
            .WithMany()
            .HasForeignKey(r => r.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure PatientRehabilitation entity
        modelBuilder.Entity<PatientRehabilitation>()
            .HasKey(r => r.Id);

        modelBuilder.Entity<PatientRehabilitation>()
            .HasOne(r => r.Patient)
            .WithMany()
            .HasForeignKey(r => r.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PatientRehabilitation>()
            .HasMany(r => r.Exercises)
            .WithOne(e => e.Rehabilitation)
            .HasForeignKey(e => e.RehabilitationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure RehabilitationExercise entity
        modelBuilder.Entity<RehabilitationExercise>()
            .HasKey(e => e.Id);
    }
}