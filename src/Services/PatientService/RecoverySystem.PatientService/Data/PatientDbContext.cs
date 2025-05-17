// Data/PatientDbContext.cs
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
            .HasForeignKey(v => v.PatientId);

        modelBuilder.Entity<Patient>()
            .HasMany(p => p.Notes)
            .WithOne(n => n.Patient)
            .HasForeignKey(n => n.PatientId);

        // Configure PatientVital entity
        modelBuilder.Entity<PatientVital>()
            .HasKey(v => v.Id);

        // Configure PatientNote entity
        modelBuilder.Entity<PatientNote>()
            .HasKey(n => n.Id);

        // Convert List<string> to JSON
        modelBuilder.Entity<Patient>()
            .Property(p => p.Medications)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
    }
}