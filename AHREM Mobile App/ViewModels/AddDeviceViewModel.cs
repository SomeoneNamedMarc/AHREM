using AHREM_Mobile_App.Models;
using AHREM_Mobile_App.Services;
using System.Windows.Input;

namespace AHREM_Mobile_App.ViewModels;

public partial class AddDeviceViewModel : BaseViewModel
{
    public ICommand AddDeviceCommand { get; }


    public AddDeviceViewModel()
    {
        AddDeviceCommand = new Command(async () => await OnAddDeviceClicked());

    }

    private async Task OnAddDeviceClicked()
    {/*
        DeviceService.AddDevice(new DeviceModel
        {
            RoomName = string.IsNullOrWhiteSpace(DeviceName) ? null : DeviceName,
            Status = "Inactive",
            PPMThresh = PPMThreshold == 0 ? null : (int?)PPMThreshold,
            TempThresh = TempThreshold == 0 ? null : (int?)TempThreshold,
            HumThresh = HumidityThreshold == 0 ? null : (int?)HumidityThreshold
        });*/

        await Shell.Current.GoToAsync("//Dashboard");
    }

    private string _deviceName;
    public string DeviceName
    {
        get => _deviceName;
        set => SetProperty(ref _deviceName, value);
    }

    private double _PPMThreshold;
    public double PPMThreshold
    {
        get => _PPMThreshold;
        set
        {
            SetProperty(ref _PPMThreshold, value);
            OnPropertyChanged(nameof(PPMThresholdText));
        }
    }

    private double _tempThreshold;
    public double TempThreshold
    {
        get => _tempThreshold;
        set
        {
            SetProperty(ref _tempThreshold, value);
            OnPropertyChanged(nameof(TempThresholdText));
        }
    }

    private double _humidityThreshold;
    public double HumidityThreshold
    {
        get => _humidityThreshold;
        set
        {
            SetProperty(ref _humidityThreshold, value);
            OnPropertyChanged(nameof(HumidityThresholdText));
        }
    }

    public string PPMThresholdText => $"PM Threshold: {Math.Round(PPMThreshold)}";
    public string TempThresholdText => $"Temp Threshold: {Math.Round(TempThreshold)}";
    public string HumidityThresholdText => $"Humidity Threshold: {Math.Round(HumidityThreshold)}";
}
