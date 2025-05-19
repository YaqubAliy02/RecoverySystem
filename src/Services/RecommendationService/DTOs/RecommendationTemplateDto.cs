using RecoverySystem.RecommendationService.Models;
using System;
using System.Collections.Generic;

namespace RecoverySystem.RecommendationService.DTOs
{
    public class RecommendationTemplateDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public RecommendationType Type { get; set; }
        public string Instructions { get; set; }
        public List<string> Tags { get; set; }
        public bool IsActive { get; set; }
        public Guid CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateRecommendationTemplateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public RecommendationType Type { get; set; }
        public string Instructions { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }

    public class UpdateRecommendationTemplateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public RecommendationType Type { get; set; }
        public string Instructions { get; set; }
        public List<string> Tags { get; set; }
        public bool IsActive { get; set; }
    }
}