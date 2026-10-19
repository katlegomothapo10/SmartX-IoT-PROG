using SmartX.API.Models;

namespace SmartX.API.Services;

public class RealTimeCommandService
{
    public Queue<TelemetryMessage> TelemetryQueue { get; } = new();
    public PriorityQueue<TelemetryMessage, int> CriticalAlertQueue { get; } = new();
    public Stack<CommandRequest> CommandUndoStack { get; } = new();
    public Dictionary<string, Sensor> DeviceRegistry { get; } = new();
    public SortedDictionary<DateTime, TelemetryMessage> SensorTimeline { get; } = new();
    public HashSet<string> DisconnectedNodes { get; } = new();

    public List<string> UserSearchHistory { get; } = new();
    public List<CommandRequest> CommandHistory { get; } = new();

    public void RegisterDevice(Sensor sensor) => DeviceRegistry[sensor.MacAddress] = sensor;

    public void EnqueueTelemetry(TelemetryMessage msg)
    {
        TelemetryQueue.Enqueue(msg);
        SensorTimeline[msg.ReceivedAt] = msg;
        if (msg.Priority == 0) CriticalAlertQueue.Enqueue(msg, 0);
    }

    public void LogDisconnect(string mac) => DisconnectedNodes.Add(mac);
    public void RemoveDisconnect(string mac) => DisconnectedNodes.Remove(mac);

    public void IssueCommand(CommandRequest cmd)
    {
        CommandHistory.Add(cmd);
        CommandUndoStack.Push(cmd);
    }

    public CommandRequest? UndoLastCommand()
    {
        if (CommandUndoStack.Count > 0)
        {
            var cmd = CommandUndoStack.Pop();
            cmd.IsUndone = true;
            cmd.Status = CommandStatus.Undone;
            return cmd;
        }
        return null;
    }

    public void LogSearch(string query)
    {
        if (!string.IsNullOrWhiteSpace(query))
        {
            UserSearchHistory.Add(query);
            if (UserSearchHistory.Count > 50) UserSearchHistory.RemoveAt(0);
        }
    }
}