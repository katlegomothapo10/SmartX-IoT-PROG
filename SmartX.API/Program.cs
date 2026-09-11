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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowBlazor");
app.UseHttpsRedirection();

var sensorService = app.Services.GetRequiredService<SensorDataService>();
DataSeeder.SeedSensors(sensorService);


app.MapPost("/api/sensors", (Sensor sensor, SensorDataService service) =>
{
    try
    {
        service.RegisterSensor(sensor);
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
    return Results.Ok(new
    {
        Success = true,
        Count = sensors.Count,
        Sensors = sensors
    });
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
    return Results.Ok(new
    {
        Success = true,
        Count = history.Count,
        Data = history.TakeLast(100)
    });
});

app.MapPost("/api/telemetry/bulk", (SensorDataService service) =>
{
    var count = service.SimulateBulkTelemetry();
    return Results.Ok(new
    {
        Success = true,
        Message = $"Simulated {count} telemetry packets",
        Count = count
    });
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

app.Run();