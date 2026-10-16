using System.Text.Json.Serialization;

namespace SmartX.API.Models;

public enum MessagePriority
{
    Low = 3,
    Normal = 2,
    High = 1,
    Critical = 0
}

public class TelemetryMessage
{
    [JsonPropertyName("messageId")]
    public string MessageId { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("deviceMac")]
    public string DeviceMac { get; set; } = string.Empty;

    [JsonPropertyName("sensorType")]
    public SensorType SensorType { get; set; }

    [JsonPropertyName("value")]
    public object? Value { get; set; }

    [JsonPropertyName("priority")]
    public MessagePriority Priority { get; set; } = MessagePriority.Normal;

    [JsonPropertyName("receivedAt")]
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("processedAt")]
    public DateTime? ProcessedAt { get; set; }

    [JsonPropertyName("processingNotes")]
    public string? ProcessingNotes { get; set; }

    public override string ToString()
    {
        return $"[{ReceivedAt:HH:mm:ss}] {Priority} | {DeviceMac} = {Value}";
    }
}