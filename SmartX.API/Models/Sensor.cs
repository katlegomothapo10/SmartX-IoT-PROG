using System.Text.Json.Serialization;

namespace SmartX.API.Models;


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
    public SensorStatus Status { get; set; } = SensorStatus.Active;

    [JsonPropertyName("configurationFilePath")]
    public string? ConfigurationFilePath { get; set; }

    [JsonPropertyName("registeredAt")]
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("lastSeen")]
    public DateTime? LastSeen { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    
    public static double operator +(Sensor a, Sensor b)
    {
        return a.GetCurrentValue() + b.GetCurrentValue();
    }

   
    public static double operator -(Sensor a, Sensor b)
    {
        return a.GetCurrentValue() - b.GetCurrentValue();
    }

    public static bool operator >(Sensor a, Sensor b)
    {
        return a.GetCurrentValue() > b.GetCurrentValue();
    }

    public static bool operator <(Sensor a, Sensor b)
    {
        return a.GetCurrentValue() < b.GetCurrentValue();
    }

    public static bool operator >=(Sensor a, Sensor b)
    {
        return a.GetCurrentValue() >= b.GetCurrentValue();
    }

    public static bool operator <=(Sensor a, Sensor b)
    {
        return a.GetCurrentValue() <= b.GetCurrentValue();
    }

    public static double operator *(Sensor a, double multiplier)
    {
        return a.GetCurrentValue() * multiplier;
    }

    public static double operator *(double multiplier, Sensor a)
    {
        return a.GetCurrentValue() * multiplier;
    }

    public static double operator /(Sensor a, double divisor)
    {
        if (divisor == 0) return 0;
        return a.GetCurrentValue() / divisor;
    }

  
    private double GetCurrentValue()
    {
        return Category switch
        {
            SensorType.Environmental => 25.0 + Random.Shared.NextDouble() * 10,
            SensorType.PowerConsumption => 100 + Random.Shared.Next(0, 500),
            SensorType.Actuator => Random.Shared.Next(0, 2),
            _ => 0
        };
    }

    public override string ToString()
    {
        return $"[{MacAddress}] {Category} @ {DeploymentLocation} - {Status}";
    }
}