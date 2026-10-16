using SmartX.API.Models;
using System.Collections.Concurrent;

namespace SmartX.API.Services;

public class RealTimeCommandService
{
    // ============================================================
    // DATA STRUCTURE 1: QUEUE<T> — FIFO telemetry processing
    // ============================================================
    private readonly Queue<TelemetryMessage> _telemetryQueue = new();

    // ============================================================
    // DATA STRUCTURE 2: PRIORITY QUEUE — critical alerts skip the line
    // ============================================================
    private readonly PriorityQueue<TelemetryMessage, int> _criticalQueue = new();

    // ============================================================
    // DATA STRUCTURE 3: STACK<T> — command history for Undo
    // ============================================================
    private readonly Stack<CommandRequest> _commandHistory = new();

    // ============================================================
    // DATA STRUCTURE 4: DICTIONARY — O(1) device lookup by MAC
    // ============================================================
    private readonly ConcurrentDictionary<string, Sensor> _deviceLookup = new();

    // ============================================================
    // DATA STRUCTURE 5: SORTED DICTIONARY — timeline logs by timestamp
    // ============================================================
    private readonly SortedDictionary<DateTime, List<TelemetryMessage>> _timelineLog = new();

    // ============================================================
    // DATA STRUCTURE 6: HASHSET — unique disconnected nodes
    // ============================================================
    private readonly HashSet<string> _disconnectedNodes = new();

    // ============================================================
    // LOCKING + COUNTERS
    // ============================================================
    private readonly object _queueLock = new();
    private readonly object _timelineLock = new();
    private int _totalProcessed = 0;
    private int _criticalProcessed = 0;

    // ============================================================
    // DEVICE REGISTRATION (feeds the Dictionary)
    // ============================================================
    public void RegisterDevice(Sensor sensor)
    {
        if (string.IsNullOrWhiteSpace(sensor.MacAddress)) return;
        _deviceLookup[sensor.MacAddress] = sensor;
    }

    public void RegisterDevices(IEnumerable<Sensor> sensors)
    {
        foreach (var s in sensors) RegisterDevice(s);
    }

    public Sensor? LookupDevice(string mac)
    {
        return _deviceLookup.TryGetValue(mac, out var sensor) ? sensor : null;
    }

    public Dictionary<string, Sensor> GetAllDevices()
    {
        return new Dictionary<string, Sensor>(_deviceLookup);
    }

    // ============================================================
    // TELEMETRY INGESTION — feeds Queue or PriorityQueue
    // ============================================================
    public TelemetryMessage EnqueueTelemetry(TelemetryMessage message)
    {
        // Determine priority from sensor type + value
        message.Priority = DeterminePriority(message);

        lock (_queueLock)
        {
            if (message.Priority == MessagePriority.Critical ||
                message.Priority == MessagePriority.High)
            {
                // Critical / High → PriorityQueue (lower number = higher priority)
                int priorityWeight = (int)message.Priority;
                _criticalQueue.Enqueue(message, priorityWeight);
            }
            else
            {
                // Normal / Low → standard FIFO Queue
                _telemetryQueue.Enqueue(message);
            }
        }

        // Also add to the sorted timeline log
        AddToTimeline(message);

        // Track disconnected nodes if applicable
        TrackDisconnectState(message);

        return message;
    }

    private MessagePriority DeterminePriority(TelemetryMessage msg)
    {
        double value = ToDouble(msg.Value);

        if (msg.SensorType == SensorType.Environmental)
        {
            if (value < 0 || value > 40) return MessagePriority.Critical;
            if (value < 5 || value > 35) return MessagePriority.High;
        }

        if (msg.SensorType == SensorType.PowerConsumption)
        {
            if (value > 800) return MessagePriority.Critical;
            if (value > 500) return MessagePriority.High;
        }

        return MessagePriority.Normal;
    }

    // ============================================================
    // PROCESSING — drains PriorityQueue first, then Queue
    // ============================================================
    public List<TelemetryMessage> ProcessBatch(int maxCount = 20)
    {
        var processed = new List<TelemetryMessage>();

        lock (_queueLock)
        {
            // 1. Drain critical first
            while (processed.Count < maxCount && _criticalQueue.Count > 0)
            {
                var msg = _criticalQueue.Dequeue();
                msg.ProcessedAt = DateTime.UtcNow;
                msg.ProcessingNotes = "Processed via PRIORITY queue";
                processed.Add(msg);
                _criticalProcessed++;
                _totalProcessed++;
            }

            // 2. Then drain normal
            while (processed.Count < maxCount && _telemetryQueue.Count > 0)
            {
                var msg = _telemetryQueue.Dequeue();
                msg.ProcessedAt = DateTime.UtcNow;
                msg.ProcessingNotes = "Processed via FIFO queue";
                processed.Add(msg);
                _totalProcessed++;
            }
        }

        return processed;
    }

    public QueueStats GetQueueStats()
    {
        lock (_queueLock)
        {
            return new QueueStats
            {
                StandardQueueCount = _telemetryQueue.Count,
                PriorityQueueCount = _criticalQueue.Count,
                TotalProcessed = _totalProcessed,
                CriticalProcessed = _criticalProcessed
            };
        }
    }

    // ============================================================
    // COMMAND ISSUE — pushes to Stack
    // ============================================================
    public CommandRequest IssueCommand(CommandRequest command)
    {
        var device = LookupDevice(command.DeviceMac);
        if (device == null)
        {
            command.Status = CommandStatus.Failed;
            return command;
        }

        // Capture previous state for undo
        command.PreviousState = device.Status.ToString();
        command.Status = CommandStatus.Executed;
        command.IssuedAt = DateTime.UtcNow;

        // Push to history stack
        lock (_queueLock)
        {
            _commandHistory.Push(command);
        }

        // Apply effect to device state
        ApplyCommandEffect(command, device);

        return command;
    }

    private void ApplyCommandEffect(CommandRequest cmd, Sensor device)
    {
        switch (cmd.CommandType)
        {
            case CommandType.TurnOn:
                device.Status = SensorStatus.Active;
                break;
            case CommandType.TurnOff:
                device.Status = SensorStatus.Inactive;
                break;
            case CommandType.EmergencyShutdown:
                device.Status = SensorStatus.Disconnected;
                break;
            case CommandType.Reset:
                device.Status = SensorStatus.Active;
                break;
            default:
                break;
        }
    }

    // ============================================================
    // UNDO — pops from Stack
    // ============================================================
    public CommandRequest? UndoLastCommand()
    {
        lock (_queueLock)
        {
            if (_commandHistory.Count == 0) return null;

            var last = _commandHistory.Pop();
            last.IsUndone = true;
            last.Status = CommandStatus.Undone;

            // Restore previous state
            var device = LookupDevice(last.DeviceMac);
            if (device != null && !string.IsNullOrEmpty(last.PreviousState))
            {
                if (Enum.TryParse<SensorStatus>(last.PreviousState, out var restored))
                {
                    device.Status = restored;
                }
            }

            return last;
        }
    }

    public List<CommandRequest> GetCommandHistory(int maxCount = 50)
    {
        lock (_queueLock)
        {
            return _commandHistory.Take(maxCount).ToList();
        }
    }

    public CommandRequest? PeekLastCommand()
    {
        lock (_queueLock)
        {
            return _commandHistory.Count > 0 ? _commandHistory.Peek() : null;
        }
    }

    // ============================================================
    // TIMELINE — sorted dictionary operations
    // ============================================================
    private void AddToTimeline(TelemetryMessage msg)
    {
        lock (_timelineLock)
        {
            var key = new DateTime(
                msg.ReceivedAt.Year,
                msg.ReceivedAt.Month,
                msg.ReceivedAt.Day,
                msg.ReceivedAt.Hour,
                msg.ReceivedAt.Minute,
                msg.ReceivedAt.Second);

            if (!_timelineLog.ContainsKey(key))
            {
                _timelineLog[key] = new List<TelemetryMessage>();
            }
            _timelineLog[key].Add(msg);
        }
    }

    public List<TelemetryMessage> GetTimeline(DateTime? from = null, DateTime? to = null, int max = 100)
    {
        lock (_timelineLock)
        {
            var query = _timelineLog.AsEnumerable();

            if (from.HasValue)
                query = query.Where(kvp => kvp.Key >= from.Value);

            if (to.HasValue)
                query = query.Where(kvp => kvp.Key <= to.Value);

            return query
                .OrderBy(kvp => kvp.Key)
                .SelectMany(kvp => kvp.Value)
                .Take(max)
                .ToList();
        }
    }

    // ============================================================
    // DISCONNECT TRACKING — HashSet
    // ============================================================
    private void TrackDisconnectState(TelemetryMessage msg)
    {
        if (msg.Priority == MessagePriority.Critical)
        {
            lock (_queueLock)
            {
                _disconnectedNodes.Add(msg.DeviceMac);
            }
        }
    }

    public bool AddDisconnectedNode(string mac)
    {
        lock (_queueLock)
        {
            return _disconnectedNodes.Add(mac);
        }
    }

    public bool RemoveDisconnectedNode(string mac)
    {
        lock (_queueLock)
        {
            return _disconnectedNodes.Remove(mac);
        }
    }

    public List<string> GetDisconnectedNodes()
    {
        lock (_queueLock)
        {
            return _disconnectedNodes.ToList();
        }
    }

    public bool IsDisconnected(string mac)
    {
        lock (_queueLock)
        {
            return _disconnectedNodes.Contains(mac);
        }
    }

    // ============================================================
    // SEARCH / FILTER
    // ============================================================
    public List<Sensor> SearchDevices(string? query, SensorType? category, SensorStatus? status)
    {
        var devices = _deviceLookup.Values.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var q = query.ToLower();
            devices = devices.Where(d =>
                d.MacAddress.ToLower().Contains(q) ||
                d.DeploymentLocation.ToLower().Contains(q) ||
                d.Description.ToLower().Contains(q));
        }

        if (category.HasValue)
            devices = devices.Where(d => d.Category == category.Value);

        if (status.HasValue)
            devices = devices.Where(d => d.Status == status.Value);

        return devices.ToList();
    }

    // ============================================================
    // HELPERS
    // ============================================================
    private double ToDouble(object? value)
    {
        if (value == null) return 0;
        if (value is bool b) return b ? 1 : 0;
        if (value is System.Text.Json.JsonElement je)
        {
            if (je.ValueKind == System.Text.Json.JsonValueKind.Number) return je.GetDouble();
            if (je.ValueKind == System.Text.Json.JsonValueKind.True) return 1;
            if (je.ValueKind == System.Text.Json.JsonValueKind.False) return 0;
        }
        return double.TryParse(value.ToString(), out var d) ? d : 0;
    }

    // ============================================================
    // SIMULATION — for testing
    // ============================================================
    public int SimulateIncomingTraffic(int count = 50)
    {
        var macs = _deviceLookup.Keys.ToList();
        if (macs.Count == 0) return 0;

        var random = new Random();
        int produced = 0;

        for (int i = 0; i < count; i++)
        {
            var mac = macs[random.Next(macs.Count)];
            var device = _deviceLookup[mac];

            object value = device.Category switch
            {
                SensorType.Environmental => Math.Round(15.0 + random.NextDouble() * 35, 2),
                SensorType.PowerConsumption => random.Next(50, 950),
                SensorType.Actuator => random.Next(0, 2) == 1,
                _ => 0
            };

            var msg = new TelemetryMessage
            {
                DeviceMac = mac,
                SensorType = device.Category,
                Value = value
            };

            EnqueueTelemetry(msg);
            produced++;
        }

        return produced;
    }
}

public class QueueStats
{
    public int StandardQueueCount { get; set; }
    public int PriorityQueueCount { get; set; }
    public int TotalProcessed { get; set; }
    public int CriticalProcessed { get; set; }
}