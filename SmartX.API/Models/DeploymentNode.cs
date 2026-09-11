using System.Text.Json.Serialization;

namespace SmartX.API.Models;

public class DeploymentNode
{
    [JsonPropertyName("nodeId")]
    public string NodeId { get; set; } = string.Empty;

    [JsonPropertyName("zoneId")]
    public string ZoneId { get; set; } = string.Empty;

    [JsonPropertyName("facilityId")]
    public string FacilityId { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("devices")]
    public List<string> Devices { get; set; } = new();

    [JsonPropertyName("children")]
    public List<DeploymentNode> Children { get; set; } = new();

    [JsonPropertyName("isConfigured")]
    public bool IsConfigured { get; set; }

    [JsonPropertyName("lastValidated")]
    public DateTime LastValidated { get; set; }

    public DeploymentNode()
    {
        Children = new List<DeploymentNode>();
        Devices = new List<string>();
        IsConfigured = false;
        LastValidated = DateTime.UtcNow;
    }

    public DeploymentNode(string nodeId, string zoneId, string facilityId, string name)
    {
        NodeId = nodeId;
        ZoneId = zoneId;
        FacilityId = facilityId;
        Name = name;
        Children = new List<DeploymentNode>();
        Devices = new List<string>();
        IsConfigured = true;
        LastValidated = DateTime.UtcNow;
    }

    public void AddChild(DeploymentNode child)
    {
        Children.Add(child);
    }

    public void AddDevice(string deviceMac)
    {
        if (!Devices.Contains(deviceMac))
        {
            Devices.Add(deviceMac);
        }
    }

    public bool RemoveDevice(string deviceMac)
    {
        return Devices.Remove(deviceMac);
    }

    public int GetTotalDevices()
    {
        var count = Devices.Count;

        foreach (var child in Children)
        {
            count += child.GetTotalDevices();
        }

        return count;
    }

    public override string ToString()
    {
        return $"{Name} ({NodeId}) - {Devices.Count} devices, {Children.Count} children";
    }
}