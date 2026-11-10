using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SmartX.API.Models;
using SmartX.API.Services;
using SmartX.API.Utils;

var builder = WebApplication.CreateBuilder(args);

// 1. CORS Setup
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// 2. Swagger Setup with JWT Support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token."
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// 3. JWT Authentication Configuration
var jwtKey = "ThisIsASuperSecretKeyForSmartXPart2Authentication!!";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();

// 4. Dependency Injection (Services)
builder.Services.AddSingleton<SensorDataService>();
builder.Services.AddSingleton<DeploymentValidator>();
builder.Services.AddSingleton<RealTimeCommandService>();
builder.Services.AddSingleton<PredictiveEngine>();
builder.Services.AddHostedService<TelemetryBackgroundService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowBlazor");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

var sensorService = app.Services.GetRequiredService<SensorDataService>();
DataSeeder.SeedSensors(sensorService);

// --- AUTHENTICATION ---
app.MapPost("/api/auth/login", (LoginRequest req) => {
    if (string.IsNullOrEmpty(req.Email) || req.Password.Length < 4)
        return Results.Unauthorized();

    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes(jwtKey);
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Email, req.Email) }),
        Expires = DateTime.UtcNow.AddHours(2),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);

    return Results.Ok(new { Token = tokenHandler.WriteToken(token), Email = req.Email });
});

// --- PART 1 ENDPOINTS ---
app.MapPost("/api/sensors", (Sensor sensor, SensorDataService service) => {
    try { service.RegisterSensor(sensor); return Results.Ok(sensor); }
    catch (Exception ex) { return Results.BadRequest(ex.Message); }
}).RequireAuthorization();

app.MapGet("/api/sensors", (SensorDataService service) =>
    Results.Ok(new { Sensors = service.GetAllSensors() })).RequireAuthorization();

app.MapPost("/api/telemetry", (TelemetryPacket<object> packet, SensorDataService service) => {
    service.IngestTelemetry(packet);
    return Results.Ok();
}).RequireAuthorization();

app.MapGet("/api/dashboard/stats", (SensorDataService service) =>
    Results.Ok(new { Stats = service.GetDashboardStats() })).RequireAuthorization();

app.MapPost("/api/telemetry/bulk", (SensorDataService service) =>
    Results.Ok(new { Count = service.SimulateBulkTelemetry() })).RequireAuthorization();

// --- PART 2 ENDPOINTS ---
app.MapPost("/api/commands/issue", (CommandRequest req, RealTimeCommandService svc) => {
    svc.IssueCommand(req);
    return Results.Ok();
}).RequireAuthorization();

app.MapPost("/api/commands/undo", (RealTimeCommandService svc) => {
    var undone = svc.UndoLastCommand();
    return undone != null ? Results.Ok() : Results.BadRequest();
}).RequireAuthorization();

app.MapGet("/api/commands/history", (RealTimeCommandService svc) =>
    Results.Ok(svc.CommandHistory.TakeLast(20))).RequireAuthorization();

app.MapGet("/api/insights", (PredictiveEngine engine) =>
    Results.Ok(engine.GenerateSuggestions())).RequireAuthorization();

app.MapPost("/api/search", (SearchRequest req, RealTimeCommandService svc) => {
    svc.LogSearch(req.Query);
    return Results.Ok();
}).RequireAuthorization();

app.MapGet("/api/stats/realtime", (RealTimeCommandService svc) => Results.Ok(new
{
    QueueCount = svc.TelemetryQueue.Count,
    CriticalCount = svc.CriticalAlertQueue.Count,
    UndoCount = svc.CommandUndoStack.Count,
    DisconnectedCount = svc.DisconnectedNodes.Count
})).RequireAuthorization();

app.MapPost("/api/simulate/critical", (RealTimeCommandService svc) => {
    svc.EnqueueTelemetry(new TelemetryMessage
    {
        DeviceMac = "AA:BB:CC:DD:EE:02",
        SensorType = SensorType.Environmental,
        Value = 45.5,
        Priority = 0
    });
    return Results.Ok();
}).RequireAuthorization();

app.MapPost("/api/simulate/disconnect", (RealTimeCommandService svc) => {
    bool isNewAlert = svc.HandleDisconnect("AA:BB:CC:DD:EE:03");

    if (isNewAlert)
    {
        svc.EnqueueTelemetry(new TelemetryMessage
        {
            DeviceMac = "AA:BB:CC:DD:EE:03",
            SensorType = SensorType.Environmental,
            Value = 0,
            Priority = 0
        });
        return Results.Ok(new { Message = "New disconnect alert generated." });
    }
    else
    {
        return Results.Ok(new { Message = "Duplicate disconnect ignored by HashSet." });
    }
}).RequireAuthorization();

app.Run();