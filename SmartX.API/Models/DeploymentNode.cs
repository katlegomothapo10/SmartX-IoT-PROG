namespace SmartX.API.Models;

public class DeploymentNode
{
    public string NodeId { get; set; } = string.Empty;
    public string ZoneId { get; set; } = string.Empty;
    public string FacilityId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<string> Devices { get; set; } = new();
    public List<DeploymentNode> Children { get; set; } = new();
    public bool IsConfigured { get; set; } = true;
    public DateTime LastValidated { get; set; } = DateTime.UtcNow;

    public DeploymentNode() { }

    public DeploymentNode(string nodeId, string zoneId, string facilityId, string name)
    {
        NodeId = nodeId;
        ZoneId = zoneId;
        FacilityId = facilityId;
        Name = name;
    }

    public void AddChild(DeploymentNode child) => Children.Add(child);
    public void AddDevice(string deviceMac) => Devices.Add(deviceMac);
    public int GetTotalDevices() => Devices.Count + Children.Sum(c => c.GetTotalDevices());
}