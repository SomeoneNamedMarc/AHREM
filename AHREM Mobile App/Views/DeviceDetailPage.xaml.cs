using AHREM_Mobile_App.Models;
using AHREM_Mobile_App.Services;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Maui.Controls;

namespace AHREM_Mobile_App.Views;
public partial class DeviceDetailPage : ContentPage
{

    public DeviceDetailPage(int deviceId, DeviceDataService dataService)
    {
        InitializeComponent();

        var viewModel = new DeviceDetailViewModel(dataService);
        viewModel.LoadDevice(deviceId);

        BindingContext = viewModel;
    }

}
