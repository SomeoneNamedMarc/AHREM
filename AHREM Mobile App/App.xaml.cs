namespace AHREM_Mobile_App;

public partial class App : Application
{
    private Window? _mainWindow;
    public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
        _mainWindow = new Window(new LoginPage(new LoginViewModel()));
        return _mainWindow;
    }
    public void NavigateToAppShell()
    {
        if (_mainWindow != null)
        {
            _mainWindow.Page = new AppShell();
        }
    }
}
