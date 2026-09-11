using System.Text.Json.Serialization;

namespace SmartX.API.Models;

public class TelemetryRecord
{
    [JsonPropertyName("deviceMac")]
    public string DeviceMac { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("dataType")]
    public string DataType { get; set; } = string.Empty;

    [JsonPropertyName("sensorType")]
    public SensorType SensorType { get; set; }

    [JsonPropertyName("value")]
    public object? Value { get; set; }

    [JsonPropertyName("metadata")]
    public Dictionary<string, object> Metadata { get; set; } = new();
}