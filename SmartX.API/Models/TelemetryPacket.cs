using System.Text.Json.Serialization;

namespace SmartX.API.Models;

public class TelemetryPacket<T>
{
    [JsonPropertyName("deviceMac")]
    public string DeviceMac { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("dataType")]
    public string DataType => typeof(T).Name;

    [JsonPropertyName("sensorType")]
    public SensorType SensorType { get; set; }

    [JsonPropertyName("value")]
    public T Value { get; set; } = default!;

    [JsonPropertyName("metadata")]
    public Dictionary<string, object> Metadata { get; set; } = new();

    [JsonPropertyName("qualityScore")]
    public double QualityScore { get; set; } = 1.0;

    public override string ToString()
    {
        return $"[{Timestamp:HH:mm:ss}] {DeviceMac} - {DataType}: {Value}";
    }
}
