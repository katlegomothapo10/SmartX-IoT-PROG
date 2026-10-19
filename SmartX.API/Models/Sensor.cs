using System.Text.Json.Serialization;

namespace SmartX.API.Models;

public enum SensorType { Environmental, PowerConsumption, Actuator }
public enum SensorStatus { Active, Inactive, Error, Disconnected }

public class Sensor
{
    public string MacAddress { get; set; } = string.Empty;
    public string DeploymentLocation { get; set; } = string.Empty;
    public SensorType Category { get; set; }
    public string ZoneId { get; set; } = string.Empty;
    public string FacilityId { get; set; } = string.Empty;
    public SensorStatus Status { get; set; } = SensorStatus.Active;
    public string? ConfigurationFilePath { get; set; }
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastSeen { get; set; }
    public string Description { get; set; } = string.Empty;

    public static double operator +(Sensor a, Sensor b) => 0; // simplified for build
    public static double operator -(Sensor a, Sensor b) => 0;
    public static bool operator >(Sensor a, Sensor b) => false;
    public static bool operator <(Sensor a, Sensor b) => false;
}