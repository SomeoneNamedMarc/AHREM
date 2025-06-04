using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace AHREM_Mobile_App.Services
{
    using AHREM_Mobile_App.Models;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using System.Text.Json.Serialization;

    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private string _token = null!; // store token here

        private const string BaseUrl = "http://10.176.69.180:5051/API"; // change to your actual API base URL

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                Console.WriteLine("Starting login request...");
                _httpClient.Timeout = TimeSpan.FromSeconds(10);

                var loginRequest = new { Email = email, Password = password };
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/Login", loginRequest);
                Console.WriteLine($"Response status: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                    return false;

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Raw response: {json}");

                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (result == null || string.IsNullOrEmpty(result.Token))
                    return false;

                _token = result.Token;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                return true;
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"Login timeout: {ex}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login exception: {ex}");
                return false;
            }
        }



        public async Task<bool> AddDeviceAsync(DeviceModel device)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/AddDevice", device);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveDeviceAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/RemoveDevice?id={id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<DeviceModel?> GetDeviceAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/GetDevice?id={id}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<DeviceModel>();
        }

        public async Task<List<DeviceModel>> GetAllDevicesAsync()
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/GetAllDevices");
            if (!response.IsSuccessStatusCode) return new List<DeviceModel>();
            return await response.Content.ReadFromJsonAsync<List<DeviceModel>>() ?? new List<DeviceModel>();
        }

        public async Task<List<DeviceData>> GetDeviceDataAsync(int? deviceId = null, string? roomName = null)
        {
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
