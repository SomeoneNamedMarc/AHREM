using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using AHREM_Mobile_App.Models;

namespace AHREM_Mobile_App.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private string _token = string.Empty;

        private const string BaseUrl = "http://10.0.2.2:5051/API";

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                Trace.WriteLine("Starting login request...");

                var loginRequest = new { Email = email, Password = password };
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/Login", loginRequest);

                Trace.WriteLine($"Response status: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Trace.WriteLine($"Login failed: {errorContent}");
                    return false;
                }

                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (result == null || string.IsNullOrEmpty(result.Token))
                {
                    Trace.WriteLine("Login failed: Empty or null token.");
                    return false;
                }

                _token = result.Token;
                return true;
            }
            catch (TaskCanceledException ex)
            {
                Trace.WriteLine($"Login timeout: {ex}");
                return false;
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Login exception: {ex}");
                return false;
            }
        }

        private void AddAuthorizationHeader()
        {
            if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            }
        }

        public async Task<bool> AddDeviceAsync(DeviceModel device)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/AddDevice", device);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveDeviceAsync(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync($"{BaseUrl}/RemoveDevice?id={id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<DeviceModel?> GetDeviceAsync(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync($"{BaseUrl}/GetDevice?id={id}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<DeviceModel>();
        }

        public async Task<List<DeviceModel>> GetAllDevicesAsync()
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync($"{BaseUrl}/GetAllDevices");
            if (!response.IsSuccessStatusCode) return new List<DeviceModel>();
            return await response.Content.ReadFromJsonAsync<List<DeviceModel>>() ?? new List<DeviceModel>();
        }

        public async Task<List<DeviceData>> GetDeviceDataAsync(int? deviceId = null, string? roomName = null)
        {
            AddAuthorizationHeader();
            string url = $"{BaseUrl}/GetDataForDevice?";
            if (deviceId.HasValue) url += $"id={deviceId.Value}";
            else if (!string.IsNullOrEmpty(roomName)) url += $"roomName={roomName}";
            else return new List<DeviceData>();

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return new List<DeviceData>();
            return await response.Content.ReadFromJsonAsync<List<DeviceData>>() ?? new List<DeviceData>();
        }
    }

    public class LoginResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;
    }
}
