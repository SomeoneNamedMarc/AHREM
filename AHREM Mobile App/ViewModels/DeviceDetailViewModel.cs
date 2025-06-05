using AHREM_Mobile_App.Models;
using AHREM_Mobile_App.Services;
using Microcharts;
using SkiaSharp;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AHREM_Mobile_App.ViewModels;

public class DeviceDetailViewModel : INotifyPropertyChanged
{
    private readonly ApiService _apiService;
    private readonly int _deviceId;

    private Command _refreshCommand;
    public Command RefreshCommand => _refreshCommand ??= new Command(async () =>
    {
        System.Diagnostics.Debug.WriteLine("REFRESH COMMAND EXECUTED");
        await RefreshDataAsync();
    });

    private Command _testDataCommand;
    public Command TestDataCommand => _testDataCommand ??= new Command(() =>
    {
        System.Diagnostics.Debug.WriteLine("TEST DATA COMMAND EXECUTED");
        LoadTestData();
    });

    private Chart _airQualityChart;
    public Chart AirQualityChart
    {
        get => _airQualityChart;
        set
        {
            _airQualityChart = value;
            OnPropertyChanged();
            System.Diagnostics.Debug.WriteLine($"CHART SET: {value != null}");
        }
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
            System.Diagnostics.Debug.WriteLine($"IsLoading: {value}");
        }
    }

    private string _chartTitle = "Air Quality Over Time";
    public string ChartTitle
    {
        get => _chartTitle;
        set
        {
            _chartTitle = value;
            OnPropertyChanged();
        }
    }

    private string _debugInfo = "Initializing...";
    public string DebugInfo
    {
        get => _debugInfo;
        set
        {
            _debugInfo = value;
            OnPropertyChanged();
            System.Diagnostics.Debug.WriteLine($"DEBUG INFO: {value}");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public DeviceDetailViewModel(int deviceId)
    {
        System.Diagnostics.Debug.WriteLine($"VIEWMODEL CONSTRUCTOR - Device ID: {deviceId}");
        _deviceId = deviceId;
        var httpClient = new HttpClient();
        _apiService = new ApiService(httpClient);

        // Create a simple test chart immediately
        CreateSimpleTestChart();
    }

    private void CreateSimpleTestChart()
    {
        try
        {
            DebugInfo = "Creating simple test chart...";
            System.Diagnostics.Debug.WriteLine("CREATING SIMPLE TEST CHART");

            var entries = new[]
            {
                new ChartEntry(50)
                {
                    Label = "10:00",
                    Color = SKColor.Parse("#2ecc71"),
                    ValueLabel = "50"
                },
                new ChartEntry(75)
                {
                    Label = "11:00",
                    Color = SKColor.Parse("#f39c12"),
                    ValueLabel = "75"
                },
                new ChartEntry(100)
                {
                    Label = "12:00",
                    Color = SKColor.Parse("#e74c3c"),
                    ValueLabel = "100"
                },
                new ChartEntry(65)
                {
                    Label = "13:00",
                    Color = SKColor.Parse("#f39c12"),
                    ValueLabel = "65"
                },
                new ChartEntry(45)
                {
                    Label = "14:00",
                    Color = SKColor.Parse("#2ecc71"),
                    ValueLabel = "45"
                }
            };

            AirQualityChart = new BarChart
            {
                Entries = entries,
                BackgroundColor = SKColors.White,
                LabelTextSize = 20,
                ValueLabelOrientation = Orientation.Horizontal,
                IsAnimated = true,
                AnimationDuration = TimeSpan.FromMilliseconds(500),
                MinValue = 0,
                MaxValue = 100,
                LabelColor = SKColors.Black,
                Margin = 20
            };

            DebugInfo = "Simple test chart created!";
        }
        catch (Exception ex)
        {
            DebugInfo = $"ERROR: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"ERROR IN SIMPLE TEST CHART: {ex}");
        }
    }

    public async Task LoadDeviceAsync()
    {
        System.Diagnostics.Debug.WriteLine("LOAD DEVICE ASYNC CALLED");
        IsLoading = true;
        DebugInfo = $"Loading device {_deviceId}...";

        try
        {
            var data = await _apiService.GetDeviceDataAsync(_deviceId);
            System.Diagnostics.Debug.WriteLine($"API DATA: {data?.Count ?? 0} records");

            if (data != null && data.Any())
            {
                DebugInfo = $"Got {data.Count} records";
                CreateAirQualityChart(data);
            }
            else
            {
                DebugInfo = "No data - using test data";
                LoadTestData();
            }
        }
        catch (Exception ex)
        {
            DebugInfo = $"Error: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"LOAD DEVICE ERROR: {ex}");
            LoadTestData();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void LoadTestData()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("LOAD TEST DATA CALLED");
            DebugInfo = "Loading test data...";

            var testData = new List<DeviceData>();
            var baseTime = DateTime.Now.AddHours(-6);

            for (int i = 0; i < 12; i++)
            {
                testData.Add(new DeviceData
                {
                    TimeStamp = baseTime.AddMinutes(i * 30),
                    AirQuality = 30 + (i * 10) % 80,
                    RoomName = "Test Room"
                });
            }

            CreateAirQualityChart(testData);
            DebugInfo = $"Test data loaded: {testData.Count} points";
        }
        catch (Exception ex)
        {
            DebugInfo = $"Test error: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"LOAD TEST DATA ERROR: {ex}");
        }
    }

    private void CreateAirQualityChart(List<DeviceData> data)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"CREATE CHART: {data.Count} items");

            var sortedData = data.OrderBy(d => d.TimeStamp).ToList();

            var entries = sortedData
                .Select(d => new ChartEntry(d.AirQuality)
                {
                    Label = d.TimeStamp.ToString("HH:mm"),  // X-axis label (Time)
                                                            // Remove ValueLabel to avoid confusion
                                                            // ValueLabel = d.AirQuality.ToString("0"), 
                    Color = GetAirQualityColor(d.AirQuality)
                })
                .ToArray();

            System.Diagnostics.Debug.WriteLine($"ENTRIES CREATED: {entries.Length}");

            AirQualityChart = new LineChart
            {
                Entries = entries,
                LineMode = LineMode.Spline,  // Curved line for better visualization
                LineSize = 3,
                PointMode = PointMode.Circle,
                PointSize = 10,
                BackgroundColor = SKColors.White,
                LabelTextSize = 20,  // Smaller text for time labels
                IsAnimated = true,
                AnimationDuration = TimeSpan.FromMilliseconds(500),
                MinValue = 0,      // Y-axis minimum
                MaxValue = 100,    // Y-axis maximum
                LabelColor = SKColors.Black,
                Margin = 20,
                // This removes the value labels from showing
                ValueLabelOrientation = Orientation.Default,
                LineAreaAlpha = 32  // Add some transparency to the filled area
            };

            if (data.Any() && !string.IsNullOrEmpty(data.First().RoomName))
            {
                ChartTitle = $"Air Quality - {data.First().RoomName}";
            }

            DebugInfo = $"Chart: {entries.Length} points (Y: Air Quality 0-100)";
        }
        catch (Exception ex)
        {
            DebugInfo = $"Chart error: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"CREATE CHART ERROR: {ex}");
        }
    }

    private SKColor GetAirQualityColor(float airQuality)
    {
        return airQuality switch
        {
            <= 50 => SKColor.Parse("#2ecc71"),
            <= 100 => SKColor.Parse("#f39c12"),
            <= 150 => SKColor.Parse("#e74c3c"),
            _ => SKColor.Parse("#c0392b")
        };
    }

    public async Task RefreshDataAsync()
    {
        System.Diagnostics.Debug.WriteLine("REFRESH DATA ASYNC CALLED");
        await LoadDeviceAsync();
    }
}