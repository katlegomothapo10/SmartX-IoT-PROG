using SmartX.API.Models;
using SmartX.API.Services;
using SmartX.API.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
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
DataSeeder.SeedSensors(sensorService);

// --- PART 1 ENDPOINTS ---
app.MapPost("/api/sensors", (Sensor sensor, SensorDataService service) => {
    try { service.RegisterSensor(sensor); return Results.Ok(sensor); }
    catch (Exception ex) { return Results.BadRequest(ex.Message); }
});
app.MapGet("/api/sensors", (SensorDataService service) => Results.Ok(new { Sensors = service.GetAllSensors() }));
app.MapPost("/api/telemetry", (TelemetryPacket<object> packet, SensorDataService service) => { service.IngestTelemetry(packet); return Results.Ok(); });
app.MapGet("/api/dashboard/stats", (SensorDataService service) => Results.Ok(new { Stats = service.GetDashboardStats() }));
app.MapPost("/api/telemetry/bulk", (SensorDataService service) => Results.Ok(new { Count = service.SimulateBulkTelemetry() }));

// --- PART 2 ENDPOINTS ---
app.MapPost("/api/auth/login", (LoginRequest req) => {
    if (req.Password.Length >= 4) return Results.Ok(new { Token = "valid-token-123", Email = req.Email });
    return Results.Unauthorized();
});

app.MapPost("/api/commands/issue", (CommandRequest req, RealTimeCommandService svc) => {
    svc.IssueCommand(req);
    return Results.Ok();
});

app.MapPost("/api/commands/undo", (RealTimeCommandService svc) => {
    var undone = svc.UndoLastCommand();
    return undone != null ? Results.Ok() : Results.BadRequest();
});

app.MapGet("/api/commands/history", (RealTimeCommandService svc) => Results.Ok(svc.CommandHistory.TakeLast(20)));
app.MapGet("/api/insights", (PredictiveEngine engine) => Results.Ok(engine.GenerateSuggestions()));

app.MapPost("/api/search", (string q, RealTimeCommandService svc) => {
    svc.LogSearch(q);
    return Results.Ok();
});

app.MapGet("/api/stats/realtime", (RealTimeCommandService svc) => Results.Ok(new
{
    QueueCount = svc.TelemetryQueue.Count,
    CriticalCount = svc.CriticalAlertQueue.Count,
    UndoCount = svc.CommandUndoStack.Count,
    DisconnectedCount = svc.DisconnectedNodes.Count
}));

app.MapPost("/api/simulate/critical", (RealTimeCommandService svc) => {
    svc.EnqueueTelemetry(new TelemetryMessage { DeviceMac = "AA:BB:CC:DD:EE:02", SensorType = SensorType.Environmental, Value = 45.5, Priority = 0 });
    return Results.Ok();
});

app.MapPost("/api/simulate/disconnect", (RealTimeCommandService svc) => {
    svc.LogDisconnect("AA:BB:CC:DD:EE:03");
    return Results.Ok();
});

app.Run();