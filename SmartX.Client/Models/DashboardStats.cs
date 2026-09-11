using System.Text.Json.Serialization;

namespace SmartX.Client.Models;

public class DashboardStats
{
    [JsonPropertyName("totalSensors")]
    public int TotalSensors { get; set; }

    [JsonPropertyName("activeSensors")]
    public int ActiveSensors { get; set; }

    [JsonPropertyName("disconnectedSensors")]
    public int DisconnectedSensors { get; set; }

    [JsonPropertyName("errorSensors")]
    public int ErrorSensors { get; set; }

    [JsonPropertyName("totalTelemetryRecords")]
    public int TotalTelemetryRecords { get; set; }

    [JsonPropertyName("totalAnomalies")]
    public int TotalAnomalies { get; set; }

    [JsonPropertyName("totalIngested")]
    public int TotalIngested { get; set; }
}

public class ApiResponse<T>
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("count")]
    public int? Count { get; set; }

    [JsonPropertyName("sensors")]
    public List<Sensor>? Sensors { get; set; }

    [JsonPropertyName("sensor")]
    public Sensor? Sensor { get; set; }

    [JsonPropertyName("data")]
    public T? Data { get; set; }

    [JsonPropertyName("stats")]
    public DashboardStats? Stats { get; set; }
}