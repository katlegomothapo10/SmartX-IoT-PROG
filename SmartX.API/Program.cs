using SmartX.API.Models;
using SmartX.API.Services;
using SmartX.API.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<SensorDataService>();
builder.Services.AddSingleton<DeploymentValidator>();
builder.Services.AddSingleton<RealTimeCommandService>();
builder.Services.AddSingleton<PredictiveEngine>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowBlazor");
app.UseHttpsRedirection();

var sensorService = app.Services.GetRequiredService<SensorDataService>();
var commandService = app.Services.GetRequiredService<RealTimeCommandService>();
DataSeeder.SeedSensors(sensorService);

// Sync Part 1 sensors into Part 2 device lookup
foreach (var sensor in sensorService.GetAllSensors())
{
    commandService.RegisterDevice(sensor);
}


app.MapPost("/api/sensors", (Sensor sensor, SensorDataService service, RealTimeCommandService cmdService) =>
{
    try
    {
        service.RegisterSensor(sensor);
        cmdService.RegisterDevice(sensor);
        return Results.Created($"/api/sensors/{sensor.MacAddress}", new
        {
            Success = true,
            Message = "Sensor registered successfully",
            Sensor = sensor
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Success = false, Message = ex.Message });
    }
});

app.MapGet("/api/sensors", (SensorDataService service) =>
{
    var sensors = service.GetAllSensors();
    return Results.Ok(new { Success = true, Count = sensors.Count, Sensors = sensors });
});

app.MapGet("/api/sensors/{macAddress}", (string macAddress, SensorDataService service) =>
{
    var sensor = service.GetSensorByMac(macAddress);
    return sensor is not null
        ? Results.Ok(new { Success = true, Sensor = sensor })
        : Results.NotFound(new { Success = false, Message = "Sensor not found" });
});

app.MapDelete("/api/sensors/{macAddress}", (string macAddress, SensorDataService service) =>
{
    var deleted = service.DeleteSensor(macAddress);
    return deleted
        ? Results.Ok(new { Success = true, Message = "Sensor deleted" })
        : Results.NotFound(new { Success = false, Message = "Sensor not found" });
});



app.MapPost("/api/telemetry", (TelemetryPacket<object> packet, SensorDataService service) =>
{
    try
    {
        service.IngestTelemetry(packet);
        return Results.Ok(new { Success = true, Message = "Telemetry ingested" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Success = false, Message = ex.Message });
    }
});

app.MapPost("/api/telemetry/float", (TelemetryPacket<float> packet, SensorDataService service) =>
{
    service.IngestTelemetryFloat(packet);
    return Results.Ok(new { Success = true, Message = "Float telemetry ingested" });
});

app.MapPost("/api/telemetry/int", (TelemetryPacket<int> packet, SensorDataService service) =>
{
    service.IngestTelemetryInt(packet);
    return Results.Ok(new { Success = true, Message = "Integer telemetry ingested" });
});

app.MapPost("/api/telemetry/bool", (TelemetryPacket<bool> packet, SensorDataService service) =>
{
    service.IngestTelemetryBool(packet);
    return Results.Ok(new { Success = true, Message = "Boolean telemetry ingested" });
});

app.MapGet("/api/telemetry/{macAddress}", (string macAddress, SensorDataService service) =>
{
    var data = service.GetLatestTelemetry(macAddress);
    return data is not null
        ? Results.Ok(new { Success = true, Data = data })
        : Results.NotFound(new { Success = false, Message = "No telemetry found" });
});

app.MapGet("/api/telemetry/{macAddress}/history", (string macAddress, SensorDataService service) =>
{
    var history = service.GetTelemetryHistory(macAddress);
    return Results.Ok(new { Success = true, Count = history.Count, Data = history.TakeLast(100) });
});

app.MapPost("/api/telemetry/bulk", (SensorDataService service) =>
{
    var count = service.SimulateBulkTelemetry();
    return Results.Ok(new { Success = true, Message = $"Simulated {count} telemetry packets", Count = count });
});

app.MapPost("/api/sensors/{macAddress}/file", async (string macAddress, IFormFile file, SensorDataService service) =>
{
    try
    {
        if (file == null || file.Length == 0)
            return Results.BadRequest(new { Success = false, Message = "No file uploaded" });

        var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        Directory.CreateDirectory(uploadsDir);
        var fileName = $"{macAddress}_{DateTime.Now:yyyyMMddHHmmss}_{file.FileName}";
        var filePath = Path.Combine(uploadsDir, fileName);

        using (var stream = File.Create(filePath))
        {
            await file.CopyToAsync(stream);
        }

        var attached = service.AttachConfigurationFile(macAddress, fileName, filePath);
        return attached
            ? Results.Ok(new { Success = true, Message = "File uploaded", FileName = fileName })
            : Results.NotFound(new { Success = false, Message = "Sensor not found" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Success = false, Message = ex.Message });
    }
}).DisableAntiforgery();

app.MapGet("/api/deployment/tree", (SensorDataService service) =>
{
    var tree = service.GetDeploymentTree();
    return Results.Ok(new { Success = true, Tree = tree });
});

app.MapPost("/api/deployment/validate", (List<DeploymentNode> tree, DeploymentValidator validator) =>
{
    var report = validator.ValidateEntireTree(tree);
    return Results.Ok(new { Success = true, Report = report });
});

app.MapGet("/api/dashboard/stats", (SensorDataService service) =>
{
    var stats = service.GetDashboardStats();
    return Results.Ok(new { Success = true, Stats = stats });
});

// ============================================================
// PART 2 ENDPOINTS — REAL-TIME COMMAND STREAM
// ============================================================

// --- Telemetry Message Enqueue ---
app.MapPost("/api/rt/telemetry/enqueue", (TelemetryMessage message, RealTimeCommandService cmd) =>
{
    var enqueued = cmd.EnqueueTelemetry(message);
    return Results.Ok(new
    {
        Success = true,
        Message = "Telemetry enqueued",
        MessageId = enqueued.MessageId,
        Priority = enqueued.Priority.ToString()
    });
});

// --- Process Batch ---
app.MapPost("/api/rt/telemetry/process", (int? maxCount, RealTimeCommandService cmd) =>
{
    var processed = cmd.ProcessBatch(maxCount ?? 20);
    return Results.Ok(new
    {
        Success = true,
        ProcessedCount = processed.Count,
        Messages = processed
    });
});

// --- Queue Stats ---
app.MapGet("/api/rt/queue/stats", (RealTimeCommandService cmd) =>
{
    var stats = cmd.GetQueueStats();
    return Results.Ok(new { Success = true, Stats = stats });
});

// --- Simulate Incoming Traffic ---
app.MapPost("/api/rt/simulate", (int? count, RealTimeCommandService cmd) =>
{
    var produced = cmd.SimulateIncomingTraffic(count ?? 50);
    var stats = cmd.GetQueueStats();
    return Results.Ok(new
    {
        Success = true,
        Produced = produced,
        QueueStats = stats
    });
});

// --- Issue Command ---
app.MapPost("/api/rt/command", (CommandRequest command, RealTimeCommandService cmd, PredictiveEngine engine) =>
{
    var issued = cmd.IssueCommand(command);

    if (issued.Status == CommandStatus.Failed)
    {
        return Results.BadRequest(new { Success = false, Message = "Device not found", Command = issued });
    }

    engine.RecordCommand(issued);

    return Results.Ok(new { Success = true, Command = issued });
});

// --- Undo Last Command ---
app.MapPost("/api/rt/command/undo", (RealTimeCommandService cmd) =>
{
    var undone = cmd.UndoLastCommand();
    return undone is not null
        ? Results.Ok(new { Success = true, Message = "Last command undone", Command = undone })
        : Results.Ok(new { Success = false, Message = "No commands to undo" });
});

// --- Peek Last Command ---
app.MapGet("/api/rt/command/peek", (RealTimeCommandService cmd) =>
{
    var last = cmd.PeekLastCommand();
    return last is not null
        ? Results.Ok(new { Success = true, Command = last })
        : Results.Ok(new { Success = false, Message = "Stack is empty" });
});

// --- Command History ---
app.MapGet("/api/rt/command/history", (int? max, RealTimeCommandService cmd) =>
{
    var history = cmd.GetCommandHistory(max ?? 50);
    return Results.Ok(new { Success = true, Count = history.Count, Commands = history });
});

// --- Timeline ---
app.MapGet("/api/rt/timeline", (DateTime? from, DateTime? to, int? max, RealTimeCommandService cmd) =>
{
    var timeline = cmd.GetTimeline(from, to, max ?? 100);
    return Results.Ok(new { Success = true, Count = timeline.Count, Timeline = timeline });
});

// --- Disconnected Nodes ---
app.MapGet("/api/rt/disconnected", (RealTimeCommandService cmd) =>
{
    var nodes = cmd.GetDisconnectedNodes();
    return Results.Ok(new { Success = true, Count = nodes.Count, Nodes = nodes });
});

app.MapPost("/api/rt/disconnected/{mac}", (string mac, RealTimeCommandService cmd) =>
{
    var added = cmd.AddDisconnectedNode(mac);
    return Results.Ok(new
    {
        Success = true,
        Added = added,
        Message = added ? "Node marked as disconnected" : "Node already in set (no duplicate added)"
    });
});

app.MapDelete("/api/rt/disconnected/{mac}", (string mac, RealTimeCommandService cmd) =>
{
    var removed = cmd.RemoveDisconnectedNode(mac);
    return Results.Ok(new { Success = true, Removed = removed });
});

// --- Search Devices ---
app.MapGet("/api/rt/search", (string? q, SensorType? category, SensorStatus? status, RealTimeCommandService cmd, PredictiveEngine engine) =>
{
    var results = cmd.SearchDevices(q, category, status);
    engine.RecordSearch(q ?? string.Empty, category?.ToString());
    return Results.Ok(new { Success = true, Count = results.Count, Devices = results });
});

// --- Predictive Suggestions ---
app.MapGet("/api/rt/suggestions", (RealTimeCommandService cmd, PredictiveEngine engine) =>
{
    var devices = cmd.GetAllDevices().Values.ToList();
    var disconnected = cmd.GetDisconnectedNodes().ToHashSet();
    var suggestions = engine.GenerateSuggestions(devices, disconnected, cmd);
    return Results.Ok(new { Success = true, Count = suggestions.Count, Suggestions = suggestions });
});

// --- Pattern Stats ---
app.MapGet("/api/rt/patterns/stats", (PredictiveEngine engine) =>
{
    var stats = engine.GetPatternStats();
    return Results.Ok(new { Success = true, Stats = stats });
});

app.MapPost("/api/rt/patterns/clear", (PredictiveEngine engine) =>
{
    engine.ClearPatterns();
    return Results.Ok(new { Success = true, Message = "Pattern history cleared" });
});

// --- Command Types Enum (for frontend dropdown) ---
app.MapGet("/api/rt/command-types", () =>
{
    var types = Enum.GetNames(typeof(CommandType));
    return Results.Ok(new { Success = true, Types = types });
});

app.Run();