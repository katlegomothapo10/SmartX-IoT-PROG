using System.Text.Json.Serialization;

namespace SmartX.Client.Models;

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
    public string CommandId { get; set; } = string.Empty;

    [JsonPropertyName("deviceMac")]
    public string DeviceMac { get; set; } = string.Empty;

    [JsonPropertyName("commandType")]
    public CommandType CommandType { get; set; }

    [JsonPropertyName("targetValue")]
    public string? TargetValue { get; set; }

    [JsonPropertyName("issuedAt")]
    public DateTime IssuedAt { get; set; }

    [JsonPropertyName("issuedBy")]
    public string IssuedBy { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public CommandStatus Status { get; set; }

    [JsonPropertyName("isUndone")]
    public bool IsUndone { get; set; }

    [JsonPropertyName("previousState")]
    public string? PreviousState { get; set; }
}