using RecoverySystem.RecommendationService.Models;

namespace RecoverySystem.RecommendationService.DTOs
{
    public class RecommendationDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public RecommendationType Type { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }
        public Guid? CaseId { get; set; }
        public RecommendationPriority Priority { get; set; }
        public RecommendationStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Instructions { get; set; }
        public List<string> Tags { get; set; }
        public Guid CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? ApprovedById { get; set; }
        public string ApprovedByName { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string Notes { get; set; }
    }

    public class CreateRecommendationDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public RecommendationType Type { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }
        public Guid? CaseId { get; set; }
        public RecommendationPriority Priority { get; set; } = RecommendationPriority.Medium;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Instructions { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public string Notes { get; set; }
    }

    public class UpdateRecommendationDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public RecommendationType Type { get; set; }
        public RecommendationPriority Priority { get; set; }
        public RecommendationStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Instructions { get; set; }
        public List<string> Tags { get; set; }
        public string Notes { get; set; }
    }

    public class ApproveRecommendationDto
    {
        public string Notes { get; set; }
    }
}