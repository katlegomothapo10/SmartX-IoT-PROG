using System.Text.Json.Serialization;

namespace SmartX.Client.Models;

public class QueueStats
{
    [JsonPropertyName("standardQueueCount")]
    public int StandardQueueCount { get; set; }

    [JsonPropertyName("priorityQueueCount")]
    public int PriorityQueueCount { get; set; }

    [JsonPropertyName("totalProcessed")]
    public int TotalProcessed { get; set; }

    [JsonPropertyName("criticalProcessed")]
    public int CriticalProcessed { get; set; }
}

public class PatternStats
{
    [JsonPropertyName("totalSearches")]
    public int TotalSearches { get; set; }

    [JsonPropertyName("totalCommands")]
    public int TotalCommands { get; set; }

    [JsonPropertyName("uniqueSearchKeys")]
    public int UniqueSearchKeys { get; set; }

    [JsonPropertyName("uniqueCommandKeys")]
    public int UniqueCommandKeys { get; set; }

    [JsonPropertyName("topSearchQuery")]
    public string TopSearchQuery { get; set; } = string.Empty;

    [JsonPropertyName("topCommandKey")]
    public string TopCommandKey { get; set; } = string.Empty;
}

public class RtResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("count")]
    public int? Count { get; set; }

    [JsonPropertyName("produced")]
    public int? Produced { get; set; }

    [JsonPropertyName("processedCount")]
    public int? ProcessedCount { get; set; }

    [JsonPropertyName("added")]
    public bool? Added { get; set; }

    [JsonPropertyName("removed")]
    public bool? Removed { get; set; }

    [JsonPropertyName("queueStats")]
    public QueueStats? QueueStats { get; set; }

    [JsonPropertyName("stats")]
    public QueueStats? Stats { get; set; }

    [JsonPropertyName("command")]
    public CommandRequest? Command { get; set; }

    [JsonPropertyName("commands")]
    public List<CommandRequest>? Commands { get; set; }

    [JsonPropertyName("devices")]
    public List<Sensor>? Devices { get; set; }

    [JsonPropertyName("nodes")]
    public List<string>? Nodes { get; set; }

    [JsonPropertyName("suggestions")]
    public List<SuggestedAction>? Suggestions { get; set; }

    [JsonPropertyName("messages")]
    public List<TelemetryMessage>? Messages { get; set; }

    [JsonPropertyName("timeline")]
    public List<TelemetryMessage>? Timeline { get; set; }

    [JsonPropertyName("types")]
    public List<string>? Types { get; set; }
}