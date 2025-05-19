using RecoverySystem.RehabilitationService.Models;
using System;
using System.Collections.Generic;

namespace RecoverySystem.RehabilitationService.DTOs
{
    public class RehabilitationSessionDto
    {
        public Guid Id { get; set; }
        public Guid RehabilitationProgramId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public SessionStatus Status { get; set; }
        public List<ActivityCompletionDto> CompletedActivities { get; set; }
        public string Notes { get; set; }
        public int PainLevel { get; set; }
        public int FatigueLevel { get; set; }
        public int SatisfactionLevel { get; set; }
        public Guid? SupervisedById { get; set; }
        public string SupervisedByName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ActivityCompletionDto
    {
        public Guid ActivityId { get; set; }
        public string ActivityTitle { get; set; }
        public bool IsCompleted { get; set; }
        public int? CompletedSets { get; set; }
        public int? CompletedRepetitions { get; set; }
        public int? DurationMinutes { get; set; }
        public string Notes { get; set; }
    }

    public class CreateRehabilitationSessionDto
    {
        public Guid RehabilitationProgramId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public string Notes { get; set; }
    }

    public class UpdateRehabilitationSessionDto
    {
        public DateTime ScheduledDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public SessionStatus Status { get; set; }
        public List<ActivityCompletionDto> CompletedActivities { get; set; }
        public string Notes { get; set; }
        public int PainLevel { get; set; }
        public int FatigueLevel { get; set; }
        public int SatisfactionLevel { get; set; }
    }
}