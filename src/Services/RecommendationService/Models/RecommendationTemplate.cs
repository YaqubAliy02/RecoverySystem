namespace RecoverySystem.RecommendationService.Models;

public class RecommendationTemplate
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public RecommendationType Type { get; set; }
    public string Instructions { get; set; }
    public List<string> Tags { get; set; } = new List<string>();
    public bool IsActive { get; set; } = true;
    public Guid CreatedById { get; set; }
    public string CreatedByName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}