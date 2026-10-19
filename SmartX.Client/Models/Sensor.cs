namespace SmartX.Client.Models;

public enum SensorType { Environmental, PowerConsumption, Actuator }
public enum SensorStatus { Active, Inactive, Error, Disconnected }

public class Sensor
{
    public string MacAddress { get; set; } = "";
    public string DeploymentLocation { get; set; } = "";
    public SensorType Category { get; set; }
    public string ZoneId { get; set; } = "";
    public string FacilityId { get; set; } = "";
    public SensorStatus Status { get; set; }
    public string? ConfigurationFilePath { get; set; }
    public DateTime RegisteredAt { get; set; }
    public DateTime? LastSeen { get; set; }
    public string Description { get; set; } = "";
}