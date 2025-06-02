using AHREM_Mobile_App.Models;
using AHREM_Mobile_App.Services;
using CommunityToolkit.Mvvm.DependencyInjection;
using System.Windows.Input;

namespace AHREM_Mobile_App.ViewModels;

public partial class DeviceListViewModel : BaseViewModel
{
    private readonly DeviceService _deviceService = new DeviceService();
    private readonly DeviceDataService _deviceDataService;

    public ObservableCollection<DeviceModel> Devices => _deviceService.GetAllDevices();

    public ICommand AddNewDeviceCommand { get; }
    public ICommand DeviceSelectedCommand { get; }

    public DeviceListViewModel(DeviceDataService deviceDataService)
    {
        _deviceDataService = deviceDataService;

        AddNewDeviceCommand = new Command(async () => await OnAddDeviceClicked());
        DeviceSelectedCommand = new Command<DeviceModel>(async (device) => await OnDeviceSelected(device));
    }

    private async Task OnDeviceSelected(DeviceModel selectedDevice)
    {
        if (selectedDevice == null) return;

        var page = new DeviceDetailPage(selectedDevice.ID, _deviceDataService);
        await Shell.Current.Navigation.PushAsync(page);
    }

    private async Task OnAddDeviceClicked()
    {
        await Shell.Current.GoToAsync("//VerifyDevice");
    }
}
