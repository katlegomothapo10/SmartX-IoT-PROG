using SmartX.API.Models;

namespace SmartX.API.Services;

public class PredictiveEngine
{
    private readonly RealTimeCommandService _svc;
    public PredictiveEngine(RealTimeCommandService svc) => _svc = svc;

    public List<SuggestedAction> GenerateSuggestions()
    {
        var actions = new List<SuggestedAction>();

        if (_svc.UserSearchHistory.Any(q => q.Contains("pump", StringComparison.OrdinalIgnoreCase)))
        {
            actions.Add(new SuggestedAction
            {
                Title = "Pump Action Required",
                Description = "You recently searched for pumps. Consider toggling the main pump.",
                Reason = "Based on recent search history.",
                TargetMac = "AA:BB:CC:DD:EE:01"
            });
        }

        foreach (var mac in _svc.DisconnectedNodes.Take(2))
        {
            actions.Add(new SuggestedAction
            {
                Title = "Device Offline",
                Description = $"Sensor {mac} is disconnected.",
                Reason = "Hardware network fault.",
                TargetMac = mac
            });
        }

        var highTemp = _svc.SensorTimeline.Values.LastOrDefault(m =>
            m.SensorType == SensorType.Environmental && Convert.ToDouble(m.Value) > 40);

        if (highTemp != null)
        {
            actions.Add(new SuggestedAction
            {
                Title = "High Temp Alert",
                Description = $"Temperature hit {highTemp.Value} on {highTemp.DeviceMac}.",
                Reason = "Exceeded safety threshold.",
                TargetMac = highTemp.DeviceMac
            });
        }
        return actions;
    }
}