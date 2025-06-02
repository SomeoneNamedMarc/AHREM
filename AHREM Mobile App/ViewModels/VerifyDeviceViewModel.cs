using AHREM_Mobile_App.Models;
using System.Windows.Input;

namespace AHREM_Mobile_App.ViewModels;

public partial class VerifyDeviceViewModel : BaseViewModel
{
    public ICommand VerifyDeviceCommand { get; }


    public VerifyDeviceViewModel()
    {
        VerifyDeviceCommand = new Command(async () => await OnVerifyDeviceClicked());

    }

    private async Task OnVerifyDeviceClicked()
    {
        // Add logic for verifying the device code

        await Shell.Current.GoToAsync("//AddDevice");
    }

    private string _deviceCode;
    public string DeviceCode
    {
        get => _deviceCode;
        set => SetProperty(ref _deviceCode, value);
    }
}
