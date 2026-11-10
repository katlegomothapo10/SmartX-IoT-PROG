using SmartX.API.Models;

namespace SmartX.API.Services;

public class PredictiveEngine
{
    private readonly RealTimeCommandService _svc;
    public PredictiveEngine(RealTimeCommandService svc) => _svc = svc;

    public List<SuggestedAction> GenerateSuggestions()
    {
        var actions = new List<SuggestedAction>();

        // 1. Real Pattern Analysis based on Search History
        if (!string.IsNullOrEmpty(_svc.LastSearchQuery) &&
            _svc.SearchCommandPatterns.TryGetValue(_svc.LastSearchQuery, out var commandCounts))
        {
            // Find the most frequently executed command after this search
            var topCommand = commandCounts.OrderByDescending(kvp => kvp.Value).FirstOrDefault();
            if (topCommand.Value > 0)
            {
                var totalCommands = commandCounts.Values.Sum();
                double confidence = (double)topCommand.Value / totalCommands;

                actions.Add(new SuggestedAction
                {
                    Title = $"Pattern Detected for '{_svc.LastSearchQuery}'",
                    Description = $"You usually execute '{topCommand.Key}' after this search.",
                    Reason = $"Based on {topCommand.Value} past occurrences ({confidence:P0} confidence).",
                    TargetMac = "Recommended Device"
                });
            }
        }

        // 2. Highlight Faulty Devices (Disconnected)
        foreach (var mac in _svc.DisconnectedNodes.Take(2))
        {
            actions.Add(new SuggestedAction
            {
                Title = "Device Offline",
                Description = $"Sensor {mac} is disconnected.",
                Reason = "Hardware network fault (Unique HashSet Alert).",
                TargetMac = mac
            });
        }

        // 3. Highlight Out-of-range readings (SortedDictionary)
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