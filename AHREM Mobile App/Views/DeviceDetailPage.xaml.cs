using AHREM_Mobile_App.ViewModels;
using Microsoft.Maui.Controls;

namespace AHREM_Mobile_App.Views;

public partial class DeviceDetailPage : ContentPage
{
    private readonly DeviceDetailViewModel _viewModel;

    public DeviceDetailPage(int deviceId)
    {
        InitializeComponent();
        _viewModel = new DeviceDetailViewModel(deviceId);
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadDeviceAsync();
    }
}