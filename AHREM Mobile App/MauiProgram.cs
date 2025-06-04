using AHREM_Mobile_App.Services;
using AHREM_Mobile_App.Views;
using Microsoft.Extensions.DependencyInjection;
using Microcharts.Maui;

namespace AHREM_Mobile_App;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
            .UseMicrocharts();
#if DEBUG
		builder.Logging.AddDebug();
#endif
		builder.Services.AddSingleton<DashboardViewModel>();
		builder.Services.AddSingleton<DashboardPage>();

        builder.Services.AddSingleton<DeviceListViewModel>();
        builder.Services.AddSingleton<DeviceListPage>();

        builder.Services.AddSingleton<DeviceDataService>();
        builder.Services.AddTransient<DeviceDetailViewModel>();
        builder.Services.AddSingleton<DeviceService>();
        builder.Services.AddTransient<DeviceDetailPage>();

        builder.Services.AddSingleton<AddDeviceViewModel>();
        builder.Services.AddSingleton<AddDevicePage>();

        builder.Services.AddSingleton<VerifyDeviceViewModel>();
        builder.Services.AddSingleton<VerifyDevicePage>();

        builder.Services.AddSingleton<SettingsViewModel>();
		builder.Services.AddSingleton<SettingsPage>();

        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<ApiService>();
        builder.Services.AddHttpClient();

        return builder.Build();
	}
}
