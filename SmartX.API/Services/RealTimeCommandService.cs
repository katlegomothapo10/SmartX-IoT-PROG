using System.Collections.Concurrent;
using SmartX.API.Models;

namespace SmartX.API.Services;

public class RealTimeCommandService
{
    // 1. FIFO Queue
    public Queue<TelemetryMessage> TelemetryQueue { get; } = new();

    // 2. Priority Queue
    public PriorityQueue<TelemetryMessage, int> CriticalAlertQueue { get; } = new();

    // 3. LIFO Stack
    public Stack<CommandRequest> CommandUndoStack { get; } = new();

    // 4. O(1) Dictionary Lookup
    public Dictionary<string, Sensor> DeviceRegistry { get; } = new();

    // 5. Sorted Dictionary (Composite Key: Timestamp + MAC to prevent overwriting)
    public SortedDictionary<(DateTime Timestamp, string Mac), TelemetryMessage> SensorTimeline { get; } = new();

    // 6. HashSet (Unique Disconnects)
    public HashSet<string> DisconnectedNodes { get; } = new();

    // --- PREDICTIVE ENGINE TRACKERS (Lecturer's Feedback) ---
    // Tracks: When searching for "Pump", how many times did they TurnOn vs TurnOff?
    public Dictionary<string, Dictionary<string, int>> SearchCommandPatterns { get; } = new();
    public string LastSearchQuery { get; set; } = "";
    public List<CommandRequest> CommandHistory { get; } = new();

    public void RegisterDevice(Sensor sensor) => DeviceRegistry[sensor.MacAddress] = sensor;

    public void EnqueueTelemetry(TelemetryMessage msg)
    {
        TelemetryQueue.Enqueue(msg);

        // Composite key prevents overwrite if two sensors send at the exact same millisecond
        SensorTimeline[(msg.ReceivedAt, msg.DeviceMac)] = msg;

        if (msg.Priority == 0)
        {
            CriticalAlertQueue.Enqueue(msg, 0);
        }
    }

    // 7. The Missing Hashset Logic (Lecturer's Feedback)
    public bool HandleDisconnect(string mac)
    {
        // If .Add returns true, it means the MAC was NOT in the set, so this is a NEW disconnect.
        if (DisconnectedNodes.Add(mac))
        {
            return true; // Should process the alert
        }
        return false; // Duplicate! Ignore the alert.
    }

    public void IssueCommand(CommandRequest cmd)
    {
        CommandHistory.Add(cmd);
        CommandUndoStack.Push(cmd);

        // 8. The Real Predictive Logic (Lecturer's Feedback)
        if (!string.IsNullOrEmpty(LastSearchQuery))
        {
            if (!SearchCommandPatterns.ContainsKey(LastSearchQuery))
                SearchCommandPatterns[LastSearchQuery] = new Dictionary<string, int>();

            var cmdTypeStr = cmd.CommandType.ToString();
            if (!SearchCommandPatterns[LastSearchQuery].ContainsKey(cmdTypeStr))
                SearchCommandPatterns[LastSearchQuery][cmdTypeStr] = 0;

            SearchCommandPatterns[LastSearchQuery][cmdTypeStr]++;
        }
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
            LastSearchQuery = query;
        }
    }
}