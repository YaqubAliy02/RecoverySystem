using System;

namespace RecoverySystem.RecommendationService.DTOs
{
    public class RecommendationFeedbackDto
    {
        public Guid Id { get; set; }
        public Guid RecommendationId { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public bool IsEffective { get; set; }
        public string Challenges { get; set; }
        public string Improvements { get; set; }
        public Guid ProvidedById { get; set; }
        public string ProvidedByName { get; set; }
        public bool IsPatientFeedback { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateRecommendationFeedbackDto
    {
        public Guid RecommendationId { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public bool IsEffective { get; set; }
        public string Challenges { get; set; }
        public string Improvements { get; set; }
        public bool IsPatientFeedback { get; set; }
    }
}