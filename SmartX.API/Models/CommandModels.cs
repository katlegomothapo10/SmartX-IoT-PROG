namespace SmartX.API.Models;

public enum CommandType { TurnOn, TurnOff, Toggle, EmergencyShutdown }
public enum CommandStatus { Pending, Executed, Undone, Failed }

public class CommandRequest
{
    public string CommandId { get; set; } = Guid.NewGuid().ToString();
    public string DeviceMac { get; set; } = string.Empty;
    public CommandType CommandType { get; set; }
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public CommandStatus Status { get; set; } = CommandStatus.Pending;
    public bool IsUndone { get; set; } = false;
}

public class TelemetryMessage
{
    public string MessageId { get; set; } = Guid.NewGuid().ToString();
    public string DeviceMac { get; set; } = string.Empty;
    public SensorType SensorType { get; set; }
    public object? Value { get; set; }
    public int Priority { get; set; } = 1;
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
}

public class SuggestedAction
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string TargetMac { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}