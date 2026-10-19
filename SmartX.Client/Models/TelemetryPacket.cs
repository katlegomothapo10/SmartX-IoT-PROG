namespace SmartX.Client.Models;

public class TelemetryRecord
{
    public string DeviceMac { get; set; } = "";
    public DateTime Timestamp { get; set; }
    public string DataType { get; set; } = "";
    public SensorType SensorType { get; set; }
    public object? Value { get; set; }
}