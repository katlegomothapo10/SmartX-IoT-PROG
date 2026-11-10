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

// --- Response Models for ApiService ---
public class LoginResponse { public string Token { get; set; } = ""; public string Email { get; set; } = ""; }
public class SensorResponse { public List<Sensor> Sensors { get; set; } = new(); }
public class StatsResponse { public DashboardStats Stats { get; set; } = new(); }
public class CountResponse { public int Count { get; set; } }
public class TelemetryResponse { public TelemetryRecord? Data { get; set; } }

// --- Part 3 Models (Graph & Tree) ---
public class MeshEdgeDto
{
    public string TargetMac { get; set; } = "";
    public int Weight { get; set; }
}

public class MstResponse
{
    public string StartNode { get; set; } = "";
    public List<MeshEdgeDto> MinimumSpanningTree { get; set; } = new();
}

public class BfsResponse
{
    public string StartNode { get; set; } = "";
    public List<string> AllConnectedNodes { get; set; } = new();
}

public class RangeQueryResponse
{
    public int Count { get; set; }
    public List<TelemetryRecord> Data { get; set; } = new();
}