using AHREM_API.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AHREM_API.Services
{
    public class DBService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public DBService(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = config["localhost:5052/api"];
        }

        public async Task<User?> GetUser(int id)
        {
            return await _httpClient.GetFromJsonAsync<User>($"{_apiBaseUrl}/users/{id}");
        }

        public async Task<User?> GetUser(string email)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/users/by-email?email={email}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<User>();
            return null;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _httpClient.GetFromJsonAsync<List<User>>($"{_apiBaseUrl}/users") ?? new List<User>();
        }

        public async Task<List<DeviceData>> GetDeviceDataForDeviceId(int deviceId)
        {
            return await _httpClient.GetFromJsonAsync<List<DeviceData>>($"{_apiBaseUrl}/devices/{deviceId}/data") ?? new List<DeviceData>();
        }

        public async Task<List<DeviceData>> GetDeviceDataForRoomName(string roomName)
        {
            return await _httpClient.GetFromJsonAsync<List<DeviceData>>($"{_apiBaseUrl}/data/by-room?roomName={roomName}") ?? new List<DeviceData>();
        }

        public async Task<Device?> GetDevice(int id)
        {
            return await _httpClient.GetFromJsonAsync<Device>($"{_apiBaseUrl}/devices/{id}");
        }

        public async Task<List<Device>> GetAllDevices()
        {
            return await _httpClient.GetFromJsonAsync<List<Device>>($"{_apiBaseUrl}/devices") ?? new List<Device>();
        }

        public async Task<bool> PostDeviceData(DeviceData deviceData)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/data", deviceData);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AddDevice(Device device)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/devices", device);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteDevice(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/devices/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CanLogin(LoginRequest loginRequest)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/auth/login", loginRequest);
            return response.IsSuccessStatusCode;
        }
    }
}
