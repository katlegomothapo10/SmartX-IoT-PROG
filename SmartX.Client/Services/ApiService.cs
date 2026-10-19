using System.Net.Http.Json;
using System.Net.Http.Headers;
using SmartX.Client.Models;

namespace SmartX.Client.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private string? _token;

    public ApiService(HttpClient http) => _http = http;

    public async Task<bool> LoginAsync(string email, string pwd)
    {
        var res = await _http.PostAsJsonAsync("api/auth/login", new { Email = email, Password = pwd });
        if (res.IsSuccessStatusCode)
        {
            var data = await res.Content.ReadFromJsonAsync<LoginResponse>();
            if (data != null && !string.IsNullOrEmpty(data.Token))
            {
                _token = data.Token;
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                return true;
            }
        }
        return false;
    }

    public async Task<List<Sensor>> GetSensorsAsync()
    {
        try { var r = await _http.GetFromJsonAsync<SensorResponse>("api/sensors"); return r?.Sensors ?? new(); } catch { return new(); }
    }

    public async Task<bool> RegisterSensorAsync(Sensor sensor)
    {
        try { var r = await _http.PostAsJsonAsync("api/sensors", sensor); return r.IsSuccessStatusCode; } catch { return false; }
    }

    public async Task<DashboardStats?> GetDashboardStatsAsync()
    {
        try { var r = await _http.GetFromJsonAsync<StatsResponse>("api/dashboard/stats"); return r?.Stats; } catch { return null; }
    }

    public async Task IssueCommandAsync(CommandRequest cmd) => await _http.PostAsJsonAsync("api/commands/issue", cmd);
    public async Task UndoCommandAsync() => await _http.PostAsync("api/commands/undo", null);
    public async Task<List<CommandRequest>?> GetCommandHistoryAsync() => await _http.GetFromJsonAsync<List<CommandRequest>>("api/commands/history");
    public async Task<List<SuggestedAction>?> GetInsightsAsync() => await _http.GetFromJsonAsync<List<SuggestedAction>>("api/insights");
    public async Task<RealtimeStats?> GetRealtimeStatsAsync() => await _http.GetFromJsonAsync<RealtimeStats>("api/stats/realtime");
    public async Task LogSearchAsync(string query) => await _http.PostAsJsonAsync($"api/search?q={query}", new { });
    public async Task SimulateCriticalAsync() => await _http.PostAsync("api/simulate/critical", null);
    public async Task SimulateDisconnectAsync() => await _http.PostAsync("api/simulate/disconnect", null);

    public async Task<TelemetryRecord?> GetLatestTelemetryAsync(string macAddress)
    {
        try { var response = await _http.GetFromJsonAsync<TelemetryResponse>($"api/telemetry/{macAddress}"); return response?.Data; } catch { return null; }
    }

    public async Task<bool> DeleteSensorAsync(string macAddress)
    {
        try { var response = await _http.DeleteAsync($"api/sensors/{macAddress}"); return response.IsSuccessStatusCode; } catch { return false; }
    }

    public async Task<int> SimulateBulkTelemetryAsync()
    {
        try { var r = await _http.PostAsync("api/telemetry/bulk", null); if (r.IsSuccessStatusCode) { var c = await r.Content.ReadFromJsonAsync<CountResponse>(); return c?.Count ?? 0; } return 0; } catch { return 0; }
    }

    public async Task<bool> UploadConfigurationFileAsync(string mac, Stream stream, string fileName)
    {
        try { using var content = new MultipartFormDataContent(); using var sc = new StreamContent(stream); content.Add(sc, "file", fileName); var r = await _http.PostAsync($"api/sensors/{mac}/file", content); return r.IsSuccessStatusCode; } catch { return false; }
    }
}

public class LoginResponse { public string Token { get; set; } = ""; public string Email { get; set; } = ""; }
public class SensorResponse { public List<Sensor> Sensors { get; set; } = new(); }
public class StatsResponse { public DashboardStats Stats { get; set; } = new(); }
public class CountResponse { public int Count { get; set; } }
public class TelemetryResponse { public TelemetryRecord? Data { get; set; } }