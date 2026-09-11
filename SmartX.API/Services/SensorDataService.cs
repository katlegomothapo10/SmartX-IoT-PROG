using SmartX.API.Models;
using System.Collections.Concurrent;

namespace SmartX.API.Services;

public class SensorDataService
{
   
    private readonly ConcurrentDictionary<string, Sensor> _sensors = new();
    private readonly ConcurrentDictionary<string, List<TelemetryRecord>> _telemetryHistory = new();
    private readonly List<DeploymentNode> _deploymentTree = new();

    private readonly double[][] _batchBuffer = new double[10][];

    private readonly double[,] _gridSummary = new double[100, 3];

    private readonly object _batchLock = new();
    private int _batchRowIndex = 0;
    private int _batchColumnIndex = 0;
    private const int MaxBatchSize = 20;
    private int _totalIngested = 0;

    public void RegisterSensor(Sensor sensor)
    {
        if (string.IsNullOrWhiteSpace(sensor.MacAddress))
            throw new ArgumentException("MAC address is required.");

        if (_sensors.ContainsKey(sensor.MacAddress))
            throw new InvalidOperationException($"Sensor {sensor.MacAddress} already registered.");

        sensor.RegisteredAt = DateTime.UtcNow;
        sensor.Status = SensorStatus.Active;

        _sensors[sensor.MacAddress] = sensor;
        _telemetryHistory[sensor.MacAddress] = new List<TelemetryRecord>();
    }

    public List<Sensor> GetAllSensors()
    {
        return _sensors.Values.ToList();
    }

    public Sensor? GetSensorByMac(string macAddress)
    {
        return _sensors.TryGetValue(macAddress, out var sensor) ? sensor : null;
    }

    public bool DeleteSensor(string macAddress)
    {
        _telemetryHistory.TryRemove(macAddress, out _);
        return _sensors.TryRemove(macAddress, out _);
    }

    public void IngestTelemetry(TelemetryPacket<object> packet)
    {
        var record = new TelemetryRecord
        {
            DeviceMac = packet.DeviceMac,
            Timestamp = packet.Timestamp,
            DataType = packet.DataType,
            SensorType = packet.SensorType,
            Value = packet.Value,
            Metadata = packet.Metadata
        };

        StoreRecord(record);
    }

    public void IngestTelemetryFloat(TelemetryPacket<float> packet)
    {
        var record = new TelemetryRecord
        {
            DeviceMac = packet.DeviceMac,
            Timestamp = packet.Timestamp,
            DataType = "Single",
            SensorType = packet.SensorType,
            Value = packet.Value,
            Metadata = packet.Metadata
        };

        StoreRecord(record);
    }

    public void IngestTelemetryInt(TelemetryPacket<int> packet)
    {
        var record = new TelemetryRecord
        {
            DeviceMac = packet.DeviceMac,
            Timestamp = packet.Timestamp,
            DataType = "Int32",
            SensorType = packet.SensorType,
            Value = packet.Value,
            Metadata = packet.Metadata
        };

        StoreRecord(record);
    }

    public void IngestTelemetryBool(TelemetryPacket<bool> packet)
    {
        var record = new TelemetryRecord
        {
            DeviceMac = packet.DeviceMac,
            Timestamp = packet.Timestamp,
            DataType = "Boolean",
            SensorType = packet.SensorType,
            Value = packet.Value,
            Metadata = packet.Metadata
        };

        StoreRecord(record);
    }

    private void StoreRecord(TelemetryRecord record)
    {
        if (!_telemetryHistory.ContainsKey(record.DeviceMac))
        {
            _telemetryHistory[record.DeviceMac] = new List<TelemetryRecord>();
        }

        _telemetryHistory[record.DeviceMac].Add(record);

        if (_sensors.TryGetValue(record.DeviceMac, out var sensor))
        {
            sensor.LastSeen = DateTime.UtcNow;
        }

        AddToBatchBuffer(record);

        UpdateGridSummary(record);

        Interlocked.Increment(ref _totalIngested);
    }

    private void AddToBatchBuffer(TelemetryRecord record)
    {
        double numericValue = ConvertToDouble(record.Value);

        lock (_batchLock)
        {
           
            if (_batchBuffer[_batchRowIndex] == null)
            {
                _batchBuffer[_batchRowIndex] = new double[MaxBatchSize];
            }

            _batchBuffer[_batchRowIndex][_batchColumnIndex] = numericValue;
            _batchColumnIndex++;

            if (_batchColumnIndex >= MaxBatchSize)
            {
                _batchColumnIndex = 0;
                _batchRowIndex++;

                if (_batchRowIndex >= _batchBuffer.Length)
                {
                    FlushBatchBuffer();
                    _batchRowIndex = 0;
                }
            }
        }
    }

    private void FlushBatchBuffer()
    {
        int nonEmptyRows = 0;
        int totalReadings = 0;

        for (int r = 0; r < _batchBuffer.Length; r++)
        {
            if (_batchBuffer[r] == null) continue;

            nonEmptyRows++;
            totalReadings += _batchBuffer[r].Length;

            // Reset the row so it can be reused
            _batchBuffer[r] = null!;
        }

        Console.WriteLine($"[BATCH FLUSH] Flushed {nonEmptyRows} rows, {totalReadings} readings.");
    }

    private void UpdateGridSummary(TelemetryRecord record)
    {
        int slot = _totalIngested % 100;
        _gridSummary[slot, 0] = ConvertToDouble(record.Value);
        _gridSummary[slot, 1] = (double)record.SensorType;
        _gridSummary[slot, 2] = IsAnomaly(record) ? 1 : 0;
    }

    private double ConvertToDouble(object? value)
    {
        return value switch
        {
            null => 0,
            bool b => b ? 1 : 0,
            float f => f,
            double d => d,
            int i => i,
            long l => l,
            _ => double.TryParse(value.ToString(), out var parsed) ? parsed : 0
        };
    }

    private bool IsAnomaly(TelemetryRecord record)
    {
        double value = ConvertToDouble(record.Value);

        return record.SensorType switch
        {
            SensorType.Environmental => value < 0 || value > 40,
            SensorType.PowerConsumption => value > 500,
            _ => false
        };
    }

    public TelemetryRecord? GetLatestTelemetry(string macAddress)
    {
        if (_telemetryHistory.TryGetValue(macAddress, out var history) && history.Count > 0)
        {
            return history[^1];
        }
        return null;
    }

    public List<TelemetryRecord> GetTelemetryHistory(string macAddress)
    {
        return _telemetryHistory.TryGetValue(macAddress, out var history)
            ? history
            : new List<TelemetryRecord>();
    }

    public bool AttachConfigurationFile(string macAddress, string fileName, string filePath)
    {
        var sensor = GetSensorByMac(macAddress);
        if (sensor == null) return false;

        sensor.ConfigurationFilePath = filePath;
        return true;
    }

    public List<DeploymentNode> GetDeploymentTree()
    {
        if (_deploymentTree.Count > 0) return _deploymentTree;

       
        var facility = new DeploymentNode("root", "facility-a", "facility-a", "Facility A");

        var zone1 = new DeploymentNode("zone-1", "zone-1", "facility-a", "Zone 1");
        var zone2 = new DeploymentNode("zone-2", "zone-2", "facility-a", "Zone 2");

        var subZoneA = new DeploymentNode("sub-a", "zone-1", "facility-a", "Sub-Zone A");
        subZoneA.AddDevice("AA:BB:CC:DD:EE:01");
        subZoneA.AddDevice("AA:BB:CC:DD:EE:02");

        var subZoneB = new DeploymentNode("sub-b", "zone-1", "facility-a", "Sub-Zone B");
        subZoneB.AddDevice("AA:BB:CC:DD:EE:03");

        var nodeHydro = new DeploymentNode("node-hydro", "zone-2", "facility-a", "Hydro Node");
        nodeHydro.AddDevice("AA:BB:CC:DD:EE:04");

        zone1.AddChild(subZoneA);
        zone1.AddChild(subZoneB);
        zone2.AddChild(nodeHydro);

        facility.AddChild(zone1);
        facility.AddChild(zone2);

        _deploymentTree.Add(facility);
        return _deploymentTree;
    }

    public int SimulateBulkTelemetry()
    {
        if (_sensors.IsEmpty) return 0;

        var macs = _sensors.Keys.ToList();
        var random = new Random();
        int produced = 0;

        for (int i = 0; i < 200; i++)
        {
            var mac = macs[random.Next(macs.Count)];
            var sensor = _sensors[mac];

            var packet = new TelemetryPacket<object>
            {
                DeviceMac = mac,
                SensorType = sensor.Category,
                Value = sensor.Category switch
                {
                    SensorType.Environmental => 20.0 + random.NextDouble() * 25,
                    SensorType.PowerConsumption => random.Next(50, 700),
                    SensorType.Actuator => random.Next(0, 2) == 1,
                    _ => 0
                },
                Timestamp = DateTime.UtcNow.AddSeconds(-random.Next(0, 300))
            };

            IngestTelemetry(packet);
            produced++;
        }

        return produced;
    }

    public object GetDashboardStats()
    {
        int active = _sensors.Values.Count(s => s.Status == SensorStatus.Active);
        int disconnected = _sensors.Values.Count(s => s.Status == SensorStatus.Disconnected);
        int error = _sensors.Values.Count(s => s.Status == SensorStatus.Error);

        int totalRecords = _telemetryHistory.Values.Sum(list => list.Count);
        int totalAnomalies = _telemetryHistory.Values
            .SelectMany(list => list)
            .Count(IsAnomaly);

        return new
        {
            TotalSensors = _sensors.Count,
            ActiveSensors = active,
            DisconnectedSensors = disconnected,
            ErrorSensors = error,
            TotalTelemetryRecords = totalRecords,
            TotalAnomalies = totalAnomalies,
            TotalIngested = _totalIngested
        };
    }
}