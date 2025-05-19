using Microsoft.EntityFrameworkCore;
using RecoverySystem.MonitoringService.Models;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text.Json;

namespace RecoverySystem.MonitoringService.Data
{
    public class MonitoringDbContext : DbContext
    {
        public MonitoringDbContext(DbContextOptions<MonitoringDbContext> options)
        : base(options)
        {
        }

        public DbSet<VitalMonitoring> VitalMonitorings { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<SystemHealth> SystemHealths { get; set; }
        public DbSet<ThresholdConfiguration> ThresholdConfigurations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure VitalMonitoring entity
            modelBuilder.Entity<VitalMonitoring>()
                .HasKey(v => v.Id);

            modelBuilder.Entity<VitalMonitoring>()
                .Property(v => v.BloodPressure)
                .HasMaxLength(20);

            modelBuilder.Entity<VitalMonitoring>()
                .Property(v => v.Source)
                .HasMaxLength(50);

            modelBuilder.Entity<VitalMonitoring>()
                .Property(v => v.DeviceId)
                .HasMaxLength(100);

            // Configure Alert entity
            modelBuilder.Entity<Alert>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<Alert>()
                .Property(a => a.PatientName)
                .HasMaxLength(200);

            modelBuilder.Entity<Alert>()
                .Property(a => a.Message)
                .HasMaxLength(500);

            modelBuilder.Entity<Alert>()
                .Property(a => a.Details)
                .HasMaxLength(2000);

            modelBuilder.Entity<Alert>()
                .Property(a => a.ResolutionNotes)
                .HasMaxLength(2000);

            // Configure SystemHealth entity
            modelBuilder.Entity<SystemHealth>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<SystemHealth>()
                .Property(s => s.ServiceName)
                .HasMaxLength(100);

            modelBuilder.Entity<SystemHealth>()
                .Property(s => s.Status)
                .HasMaxLength(50);

            modelBuilder.Entity<SystemHealth>()
                .Property(s => s.Components)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, new JsonSerializerOptions()));

            modelBuilder.Entity<SystemHealth>()
                .Property(s => s.Description)
                .HasMaxLength(1000);

            // Configure ThresholdConfiguration entity
            modelBuilder.Entity<ThresholdConfiguration>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<ThresholdConfiguration>()
                .Property(t => t.Name)
                .HasMaxLength(100);

            modelBuilder.Entity<ThresholdConfiguration>()
                .Property(t => t.VitalSign)
                .HasMaxLength(50);
        }
    }
}