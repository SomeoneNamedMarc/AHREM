using System.Windows.Input;

namespace AHREM_Mobile_App.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    public ICommand LoginCommand { get; }

    public LoginViewModel()
    {
        LoginCommand = new Command(async () => await OnLoginClicked());
    }

    private async Task OnLoginClicked()
    {
        // Add logic for authentication, later
        bool isAuthenticated = true;

        if (isAuthenticated)
        {
            // Switching to the AppShell (which contains the "internal" program)
            if (Application.Current is App app)
            {
                app.NavigateToAppShell();
            }
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", "Invalid credentials", "OK");
        }
    }
}