using RecoverySystem.RehabilitationService.Models;
using System;

namespace RecoverySystem.RehabilitationService.DTOs
{
    public class RehabilitationActivityDto
    {
        public Guid Id { get; set; }
        public Guid RehabilitationProgramId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ActivityType Type { get; set; }
        public string Instructions { get; set; }
        public int DurationMinutes { get; set; }
        public int Frequency { get; set; }
        public int Sets { get; set; }
        public int Repetitions { get; set; }
        public string VideoUrl { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateRehabilitationActivityDto
    {
        public Guid RehabilitationProgramId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ActivityType Type { get; set; }
        public string Instructions { get; set; }
        public int DurationMinutes { get; set; }
        public int Frequency { get; set; }
        public int Sets { get; set; }
        public int Repetitions { get; set; }
        public string VideoUrl { get; set; }
        public string ImageUrl { get; set; }
    }

    public class UpdateRehabilitationActivityDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public ActivityType Type { get; set; }
        public string Instructions { get; set; }
        public int DurationMinutes { get; set; }
        public int Frequency { get; set; }
        public int Sets { get; set; }
        public int Repetitions { get; set; }
        public string VideoUrl { get; set; }
        public string ImageUrl { get; set; }
    }
}