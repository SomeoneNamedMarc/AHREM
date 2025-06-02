namespace AHREM_Mobile_App.Views;

public partial class AddDevicePage : ContentPage
{
	public AddDevicePage(AddDeviceViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
