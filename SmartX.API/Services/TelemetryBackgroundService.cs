using SmartX.API.Models;

namespace SmartX.API.Services;

public class TelemetryBackgroundService : BackgroundService
{
    private readonly RealTimeCommandService _svc;
    private readonly ILogger<TelemetryBackgroundService> _logger;

    public TelemetryBackgroundService(RealTimeCommandService svc, ILogger<TelemetryBackgroundService> logger)
    {
        _svc = svc;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // 1. Process Critical Alerts FIRST (Bypass)
            while (_svc.CriticalAlertQueue.TryDequeue(out var criticalMsg, out _))
            {
                _logger.LogWarning($"[CRITICAL BYPASS] Processed {criticalMsg.DeviceMac} - Value: {criticalMsg.Value}");
            }

            // 2. Process Normal Telemetry SECOND
            while (_svc.TelemetryQueue.TryDequeue(out var normalMsg))
            {
                _logger.LogInformation($"[NORMAL QUEUE] Processed {normalMsg.DeviceMac} - Value: {normalMsg.Value}");
            }

            // Wait 1 second before checking again
            await Task.Delay(1000, stoppingToken);
        }
    }
}