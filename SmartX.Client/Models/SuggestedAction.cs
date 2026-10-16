using System.Text.Json.Serialization;

namespace SmartX.Client.Models;

public class SuggestedAction
{
    [JsonPropertyName("actionId")]
    public string ActionId { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("suggestedCommandType")]
    public CommandType SuggestedCommandType { get; set; }

    [JsonPropertyName("suggestedDeviceMac")]
    public string SuggestedDeviceMac { get; set; } = string.Empty;

    [JsonPropertyName("confidenceScore")]
    public double ConfidenceScore { get; set; }

    [JsonPropertyName("reason")]
    public string Reason { get; set; } = string.Empty;

    [JsonPropertyName("generatedAt")]
    public DateTime GeneratedAt { get; set; }
}