using AHREM_Mobile_App.Models;
using AHREM_Mobile_App.Services;

namespace AHREM_Mobile_App.ViewModels;

public class DeviceDetailViewModel : INotifyPropertyChanged
{
    private readonly DeviceDataService _dataService;

    private ObservableCollection<DeviceData> _deviceDataCollection = new();
    public ObservableCollection<DeviceData> DeviceDataCollection
    {
        get => _deviceDataCollection;
        set { _deviceDataCollection = value; OnPropertyChanged(); }
    }

    private DeviceData _latestDeviceData;
    public DeviceData LatestDeviceData
    {
        get => _latestDeviceData;
        set { _latestDeviceData = value; OnPropertyChanged(); }
    }

    public DeviceDetailViewModel(DeviceDataService dataService)
    {
        _dataService = dataService;
    }

    public void LoadDevice(int deviceId)
    {
        var allData = _dataService.GetAllData();

        // Filter all data for the specific device
        var deviceSpecificData = allData.Where(d => d.DeviceID == deviceId).ToList();

        DeviceDataCollection = new ObservableCollection<DeviceData>(deviceSpecificData);

        // Get the latest by TimeStamp
        LatestDeviceData = deviceSpecificData.OrderByDescending(d => d.TimeStamp).FirstOrDefault();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
