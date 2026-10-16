using SmartX.API.Models;
using System.Collections.Concurrent;

namespace SmartX.API.Services;

public class PredictiveEngine
{
    // Track user activity patterns
    private readonly ConcurrentDictionary<string, int> _searchPatterns = new();
    private readonly ConcurrentDictionary<string, int> _commandPatterns = new();
    private readonly ConcurrentDictionary<string, DateTime> _lastActivity = new();

    // Recent search history (last 200 searches)
    private readonly Queue<SearchRecord> _recentSearches = new();
    private const int MaxSearchHistory = 200;

    // Recent command history (last 200 commands)
    private readonly Queue<CommandRecord> _recentCommands = new();
    private const int MaxCommandHistory = 200;

    private readonly object _lock = new();

    public void RecordSearch(string query, string? category)
    {
        lock (_lock)
        {
            var key = $"{query?.ToLower() ?? "all"}|{category ?? "any"}";
            _searchPatterns.AddOrUpdate(key, 1, (_, count) => count + 1);
            _lastActivity[key] = DateTime.UtcNow;

            _recentSearches.Enqueue(new SearchRecord
            {
                Query = query ?? string.Empty,
                Category = category,
                Timestamp = DateTime.UtcNow
            });

            while (_recentSearches.Count > MaxSearchHistory)
                _recentSearches.Dequeue();
        }
    }

    public void RecordCommand(CommandRequest command)
    {
        lock (_lock)
        {
            var key = $"{command.CommandType}|{command.DeviceMac}";
            _commandPatterns.AddOrUpdate(key, 1, (_, count) => count + 1);
            _lastActivity[key] = DateTime.UtcNow;

            _recentCommands.Enqueue(new CommandRecord
            {
                CommandType = command.CommandType,
                DeviceMac = command.DeviceMac,
                Timestamp = DateTime.UtcNow
            });

            while (_recentCommands.Count > MaxCommandHistory)
                _recentCommands.Dequeue();
        }
    }

    public List<SuggestedAction> GenerateSuggestions(
        List<Sensor> currentDevices,
        HashSet<string> disconnectedNodes,
        RealTimeCommandService commandService)
    {
        var suggestions = new List<SuggestedAction>();

        // ============================================================
        // PATTERN 1: Repeated searches suggest a device of interest
        // ============================================================
        var topSearches = _searchPatterns
            .Where(kv => kv.Value >= 2)
            .OrderByDescending(kv => kv.Value)
            .Take(3)
            .ToList();

        foreach (var search in topSearches)
        {
            var parts = search.Key.Split('|');
            var query = parts[0];

            if (string.IsNullOrWhiteSpace(query) || query == "all") continue;

            var matchingDevice = currentDevices.FirstOrDefault(d =>
                d.MacAddress.ToLower().Contains(query) ||
                d.DeploymentLocation.ToLower().Contains(query) ||
                d.Description.ToLower().Contains(query));

            if (matchingDevice != null)
            {
                suggestions.Add(new SuggestedAction
                {
                    Title = $"Frequent search: {query}",
                    Description = $"You have searched for '{query}' {search.Value} times. Viewing {matchingDevice.MacAddress} now.",
                    SuggestedDeviceMac = matchingDevice.MacAddress,
                    SuggestedCommandType = CommandType.Reset,
                    ConfidenceScore = Math.Min(0.95, 0.5 + (search.Value * 0.1)),
                    Reason = $"Search pattern detected {search.Value} times"
                });
            }
        }

        // ============================================================
        // PATTERN 2: Disconnected node → suggest reset
        // ============================================================
        foreach (var mac in disconnectedNodes.Take(3))
        {
            var device = currentDevices.FirstOrDefault(d => d.MacAddress == mac);
            if (device == null) continue;

            suggestions.Add(new SuggestedAction
            {
                Title = $"Device offline: {mac}",
                Description = $"Sensor at {device.DeploymentLocation} appears offline. Try resetting it.",
                SuggestedDeviceMac = mac,
                SuggestedCommandType = CommandType.Reset,
                ConfidenceScore = 0.85,
                Reason = "Node is in the disconnected set"
            });
        }

        // ============================================================
        // PATTERN 3: Critical value detection from recent telemetry
        // ============================================================
        var recentCritical = commandService.GetTimeline(max: 50)
            .Where(m => m.Priority == MessagePriority.Critical)
            .GroupBy(m => m.DeviceMac)
            .Where(g => g.Count() >= 2)
            .Take(3);

        foreach (var group in recentCritical)
        {
            var device = currentDevices.FirstOrDefault(d => d.MacAddress == group.Key);
            if (device == null) continue;

            suggestions.Add(new SuggestedAction
            {
                Title = $"Repeated critical alerts: {group.Key}",
                Description = $"Device at {device.DeploymentLocation} has triggered {group.Count()} critical alerts recently.",
                SuggestedDeviceMac = group.Key,
                SuggestedCommandType = CommandType.EmergencyShutdown,
                ConfidenceScore = 0.9,
                Reason = "Multiple critical telemetry events detected"
            });
        }

        // ============================================================
        // PATTERN 4: Repeated commands suggest preference
        // ============================================================
        var topCommands = _commandPatterns
            .Where(kv => kv.Value >= 2)
            .OrderByDescending(kv => kv.Value)
            .Take(2)
            .ToList();

        foreach (var cmd in topCommands)
        {
            var parts = cmd.Key.Split('|');
            if (parts.Length != 2) continue;

            if (!Enum.TryParse<CommandType>(parts[0], out var cmdType)) continue;
            var mac = parts[1];

            var device = currentDevices.FirstOrDefault(d => d.MacAddress == mac);
            if (device == null) continue;

            // Skip if we've already suggested this device
            if (suggestions.Any(s => s.SuggestedDeviceMac == mac)) continue;

            suggestions.Add(new SuggestedAction
            {
                Title = $"Recurring action: {cmdType} on {mac}",
                Description = $"You have issued '{cmdType}' on this device {cmd.Value} times. Consider automating it.",
                SuggestedDeviceMac = mac,
                SuggestedCommandType = cmdType,
                ConfidenceScore = Math.Min(0.9, 0.6 + (cmd.Value * 0.05)),
                Reason = $"Repeated command pattern detected {cmd.Value} times"
            });
        }

        // ============================================================
        // PATTERN 5: Peak alert time detection
        // ============================================================
        var peakHour = _recentCommands
            .GroupBy(c => c.Timestamp.Hour)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault();

        if (peakHour != null && peakHour.Count() >= 3)
        {
            suggestions.Add(new SuggestedAction
            {
                Title = $"Peak activity at {peakHour.Key:00}:00",
                Description = $"Most command activity occurs at {peakHour.Key:00}:00. Consider pre-staging checks.",
                SuggestedCommandType = CommandType.Reset,
                ConfidenceScore = 0.7,
                Reason = "Time-based pattern detected"
            });
        }

        return suggestions
            .OrderByDescending(s => s.ConfidenceScore)
            .Take(6)
            .ToList();
    }

    public PatternStats GetPatternStats()
    {
        lock (_lock)
        {
            return new PatternStats
            {
                TotalSearches = _recentSearches.Count,
                TotalCommands = _recentCommands.Count,
                UniqueSearchKeys = _searchPatterns.Count,
                UniqueCommandKeys = _commandPatterns.Count,
                TopSearchQuery = _searchPatterns
                    .OrderByDescending(kv => kv.Value)
                    .Select(kv => kv.Key)
                    .FirstOrDefault() ?? "none",
                TopCommandKey = _commandPatterns
                    .OrderByDescending(kv => kv.Value)
                    .Select(kv => kv.Key)
                    .FirstOrDefault() ?? "none"
            };
        }
    }

    public void ClearPatterns()
    {
        lock (_lock)
        {
            _searchPatterns.Clear();
            _commandPatterns.Clear();
            _recentSearches.Clear();
            _recentCommands.Clear();
            _lastActivity.Clear();
        }
    }

    private class SearchRecord
    {
        public string Query { get; set; } = string.Empty;
        public string? Category { get; set; }
        public DateTime Timestamp { get; set; }
    }

    private class CommandRecord
    {
        public CommandType CommandType { get; set; }
        public string DeviceMac { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}

public class PatternStats
{
    public int TotalSearches { get; set; }
    public int TotalCommands { get; set; }
    public int UniqueSearchKeys { get; set; }
    public int UniqueCommandKeys { get; set; }
    public string TopSearchQuery { get; set; } = string.Empty;
    public string TopCommandKey { get; set; } = string.Empty;
}