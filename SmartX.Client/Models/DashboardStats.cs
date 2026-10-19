namespace SmartX.Client.Models;

public class DashboardStats
{
    public int TotalSensors { get; set; }
    public int ActiveSensors { get; set; }
    public int TotalTelemetryRecords { get; set; }
    public int TotalAnomalies { get; set; }
}

public class CommandRequest
{
    public string DeviceMac { get; set; } = "";
    public string CommandType { get; set; } = "TurnOn";
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public bool IsUndone { get; set; }
}

public class SuggestedAction
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Reason { get; set; } = "";
}

public class RealtimeStats
{
    public int QueueCount { get; set; }
    public int CriticalCount { get; set; }
    public int UndoCount { get; set; }
    public int DisconnectedCount { get; set; }
}

public class SensorResponse { public List<Sensor> Sensors { get; set; } = new(); }
public class StatsResponse { public DashboardStats Stats { get; set; } = new(); }
public class CountResponse { public int Count { get; set; } }