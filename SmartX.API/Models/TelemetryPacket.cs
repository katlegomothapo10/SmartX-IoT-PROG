namespace SmartX.API.Models;

public class TelemetryPacket<T>
{
    public string DeviceMac { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string DataType { get; set; } = typeof(T).Name;
    public SensorType SensorType { get; set; }
    public T Value { get; set; } = default!;
    public Dictionary<string, object> Metadata { get; set; } = new();
    public double QualityScore { get; set; } = 1.0;
}