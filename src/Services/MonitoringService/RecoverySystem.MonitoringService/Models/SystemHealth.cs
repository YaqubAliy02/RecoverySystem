namespace RecoverySystem.MonitoringService.Models;

public class SystemHealth
{
    public Guid Id { get; set; }
    public string ServiceName { get; set; }
    public string Status { get; set; } // Healthy, Degraded, Unhealthy
    public Dictionary<string, string> Components { get; set; } = new Dictionary<string, string>();
    public string Description { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}