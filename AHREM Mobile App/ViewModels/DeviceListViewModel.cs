using AHREM_Mobile_App.Models;
using AHREM_Mobile_App.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AHREM_Mobile_App.ViewModels;

public partial class DeviceListViewModel : BaseViewModel
{
    private readonly ApiService _apiService;
    private readonly DeviceDataService _deviceDataService;

    public ObservableCollection<DeviceModel> Devices { get; set; } = new();

    public ICommand DeviceSelectedCommand { get; }

    public DeviceListViewModel(ApiService apiService, DeviceDataService deviceDataService)
    {
        _apiService = apiService;
        _deviceDataService = deviceDataService;

        DeviceSelectedCommand = new Command<DeviceModel>(async (device) => await OnDeviceSelected(device));

        LoadDevicesFromApi(); // Load devices when ViewModel is constructed
    }

    private async void LoadDevicesFromApi()
    {
        try
        {
            var apiDevices = await _apiService.GetAllDevicesAsync();
            Devices.Clear();

            var random = new Random();
            string[] roomNames = { "Office", "315", "306", "300", "Bathroom", "Meeting Room", "Server Room" };

            foreach (var device in apiDevices)
            {
                // Assign random room name
                device.RoomName = roomNames[random.Next(roomNames.Length)];

                // Set default thresholds
                device.PPMThresh = 80;
                device.TempThresh = 80;
                device.HumThresh = 80;

                Devices.Add(device);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading devices: {ex.Message}");
        }
    }


    private async Task OnDeviceSelected(DeviceModel selectedDevice)
    {
        if (selectedDevice == null) return;

        var page = new DeviceDetailPage(selectedDevice.ID, _deviceDataService);
        await Shell.Current.Navigation.PushAsync(page);
    }

}
