using System.Net.Http.Json;
using SmartX.Client.Models;

namespace SmartX.Client.Services;

public class ApiService
{
    private readonly HttpClient _http;

    public ApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Sensor>> GetSensorsAsync()
    {
        try
        {
            var response = await _http.GetFromJsonAsync<ApiResponse<object>>("api/sensors");
            return response?.Sensors ?? new List<Sensor>();
        }
        catch
        {
            return new List<Sensor>();
        }
    }

    public async Task<Sensor?> GetSensorAsync(string macAddress)
    {
        try
        {
            var response = await _http.GetFromJsonAsync<ApiResponse<object>>($"api/sensors/{macAddress}");
            return response?.Sensor;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> RegisterSensorAsync(Sensor sensor)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/sensors", sensor);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteSensorAsync(string macAddress)
    {
        try
        {
            var response = await _http.DeleteAsync($"api/sensors/{macAddress}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> IngestTelemetryAsync(TelemetryPacket<object> packet)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/telemetry", packet);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<int> SimulateBulkTelemetryAsync()
    {
        try
        {
            var response = await _http.PostAsync("api/telemetry/bulk", null);
            if (!response.IsSuccessStatusCode) return 0;

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
            return result?.Count ?? 0;
        }
        catch
        {
            return 0;
        }
    }

    public async Task<TelemetryRecord?> GetLatestTelemetryAsync(string macAddress)
    {
        try
        {
            var response = await _http.GetFromJsonAsync<ApiResponse<TelemetryRecord>>($"api/telemetry/{macAddress}");
            return response?.Data;
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<TelemetryRecord>> GetTelemetryHistoryAsync(string macAddress)
    {
        try
        {
            var response = await _http.GetFromJsonAsync<ApiResponse<List<TelemetryRecord>>>($"api/telemetry/{macAddress}/history");
            return response?.Data ?? new List<TelemetryRecord>();
        }
        catch
        {
            return new List<TelemetryRecord>();
        }
    }

    public async Task<DashboardStats?> GetDashboardStatsAsync()
    {
        try
        {
            var response = await _http.GetFromJsonAsync<ApiResponse<object>>("api/dashboard/stats");
            return response?.Stats;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UploadConfigurationFileAsync(string macAddress, Stream fileStream, string fileName)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            using var streamContent = new StreamContent(fileStream);
            content.Add(streamContent, "file", fileName);

            var response = await _http.PostAsync($"api/sensors/{macAddress}/file", content);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}