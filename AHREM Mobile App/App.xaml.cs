using Microsoft.Extensions.DependencyInjection;

namespace AHREM_Mobile_App;

public partial class App : Application
{
    //private Window? _mainWindow;
    private readonly IServiceProvider _serviceProvider;
    private Window? _mainWindow;
    public App(IServiceProvider serviceProvider)
    {
		InitializeComponent();
        _serviceProvider = serviceProvider;
    }

	protected override Window CreateWindow(IActivationState? activationState)
	{
        //_mainWindow = new Window(new LoginPage(new LoginViewModel()));
        //return _mainWindow;
        var loginPage = _serviceProvider.GetRequiredService<LoginPage>();

        _mainWindow = new Window(loginPage);
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
