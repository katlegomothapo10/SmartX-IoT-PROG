using System.Text.Json.Serialization;

namespace SmartX.Client.Models;

public enum SensorType
{
    Environmental,
    PowerConsumption,
    Actuator
}

public enum SensorStatus
{
    Active,
    Inactive,
    Error,
    Disconnected
}

public class Sensor
{
    [JsonPropertyName("macAddress")]
    public string MacAddress { get; set; } = string.Empty;

    [JsonPropertyName("deploymentLocation")]
    public string DeploymentLocation { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public SensorType Category { get; set; }

    [JsonPropertyName("zoneId")]
    public string ZoneId { get; set; } = string.Empty;

    [JsonPropertyName("facilityId")]
    public string FacilityId { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public SensorStatus Status { get; set; }

    [JsonPropertyName("configurationFilePath")]
    public string? ConfigurationFilePath { get; set; }

    [JsonPropertyName("registeredAt")]
    public DateTime RegisteredAt { get; set; }

    [JsonPropertyName("lastSeen")]
    public DateTime? LastSeen { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}