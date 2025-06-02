using AHREM_Mobile_App.Models;
using AHREM_Mobile_App.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace AHREM_Mobile_App.Views;

public partial class DeviceListPage : ContentPage
{
	public DeviceListPage(DeviceListViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
    private void OnDeviceSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is DeviceModel device)
        {
            // Get the ViewModel and execute the command
            if (BindingContext is DeviceListViewModel viewModel)
            {
                viewModel.DeviceSelectedCommand.Execute(device);
            }

            // Clear selection (important for UI feedback)
            ((CollectionView)sender).SelectedItem = null;
        }
    }
}
