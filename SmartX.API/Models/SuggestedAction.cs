using System.Text.Json.Serialization;

namespace SmartX.API.Models;

public class SuggestedAction
{
    [JsonPropertyName("actionId")]
    public string ActionId { get; set; } = Guid.NewGuid().ToString();

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
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

    public override string ToString()
    {
        return $"{Title} ({ConfidenceScore:P0}) → {SuggestedDeviceMac}";
    }
}