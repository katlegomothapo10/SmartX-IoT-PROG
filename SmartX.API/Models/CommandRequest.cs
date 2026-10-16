using System.Text.Json.Serialization;

namespace SmartX.API.Models;

public enum CommandType
{
    TurnOn,
    TurnOff,
    Toggle,
    SetValue,
    Reset,
    EmergencyShutdown
}

public enum CommandStatus
{
    Pending,
    Executed,
    Undone,
    Failed
}

public class CommandRequest
{
    [JsonPropertyName("commandId")]
    public string CommandId { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("deviceMac")]
    public string DeviceMac { get; set; } = string.Empty;

    [JsonPropertyName("commandType")]
    public CommandType CommandType { get; set; }

    [JsonPropertyName("targetValue")]
    public string? TargetValue { get; set; }

    [JsonPropertyName("issuedAt")]
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("issuedBy")]
    public string IssuedBy { get; set; } = "operator";

    [JsonPropertyName("status")]
    public CommandStatus Status { get; set; } = CommandStatus.Pending;

    [JsonPropertyName("isUndone")]
    public bool IsUndone { get; set; } = false;

    [JsonPropertyName("previousState")]
    public string? PreviousState { get; set; }

    public override string ToString()
    {
        return $"[{IssuedAt:HH:mm:ss}] {CommandType} → {DeviceMac} ({Status})";
    }
}