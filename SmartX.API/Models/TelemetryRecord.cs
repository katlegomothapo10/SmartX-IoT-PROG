namespace SmartX.API.Models;

public class TelemetryRecord
{
    public string DeviceMac { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string DataType { get; set; } = string.Empty;
    public SensorType SensorType { get; set; }
    public object? Value { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}