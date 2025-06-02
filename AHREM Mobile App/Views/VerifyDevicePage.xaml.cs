namespace AHREM_Mobile_App.Views;

public partial class VerifyDevicePage : ContentPage
{
	public VerifyDevicePage(VerifyDeviceViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
