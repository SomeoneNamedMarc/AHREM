using AHREM_Mobile_App.Services;
using System.Windows.Input;

namespace AHREM_Mobile_App.ViewModels;

public partial class LoginViewModel : BaseViewModel
{

    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _hasError;
    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            SetProperty(ref _errorMessage, value);
            HasError = !string.IsNullOrWhiteSpace(value);
        }
    }
    public bool HasError
    {
        get => _hasError;
        set => SetProperty(ref _hasError, value);
    }
    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    private readonly ApiService _apiService;
    public ICommand LoginCommand { get; }

    public LoginViewModel(ApiService apiService)
    {
        _apiService = apiService;
        LoginCommand = new Command(async () => await OnLoginClicked());
    }

    private async Task OnLoginClicked()
    {
        ErrorMessage = string.Empty;
        bool isAuthenticated = await _apiService.LoginAsync(Username, Password);

        if (isAuthenticated)
        {
            if (Application.Current is App app)
            {
                app.NavigateToAppShell();
            }
        }
        else
        {
            ErrorMessage = "Invalid credentials. Please try again.";
        }
    }
}   