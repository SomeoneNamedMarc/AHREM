
using System.Diagnostics;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace AHREM_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            //builder.Services.AddAuthorization();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            //app.UseHttpsRedirection();

            //app.UseAuthorization();

            List<DeviceData> dummyData = new List<DeviceData>();
            List<Device> dummyDevices = new List<Device>();

            dummyData.AddRange(
                new DeviceData(
                    0, // deviceId
                    18.2f, // temperature
                    6.2f, // humidity
                    14.2f, // radon
                    20.1f, // PPM
                    10.5f, // airQuality
                    "300", // roomName
                    475342 // timeStamp
                ),
                new DeviceData(
                    1, // deviceId
                    18.2f, // temperature
                    6.2f, // humidity
                    14.2f, // radon
                    20.1f, // PPM
                    10.5f, // airQuality
                    "300", // roomName
                    475342 // timeStamp
                ),
                new DeviceData(
                    2, // deviceId
                    18.2f, // temperature
                    6.2f, // humidity
                    14.2f, // radon
                    20.1f, // PPM
                    10.5f, // airQuality
                    "300", // roomName
                    475342 // timeStamp
                ),
                new DeviceData(
                    3, // deviceId
                    18.2f, // temperature
                    6.2f, // humidity
                    14.2f, // radon
                    20.1f, // PPM
                    10.5f, // airQuality
                    "300", // roomName
                    475342 // timeStamp
                ),
                new DeviceData(
                    4, // deviceId
                    18.2f, // temperature
                    6.2f, // humidity
                    14.2f, // radon
                    20.1f, // PPM
                    10.5f, // airQuality
                    "300", // roomName
                    475342 // timeStamp
                ),
                new DeviceData(
                    5, // deviceId
                    18.2f, // temperature
                    6.2f, // humidity
                    14.2f, // radon
                    20.1f, // PPM
                    10.5f, // airQuality
                    "300", // roomName
                    475342 // timeStamp
                ),
                new DeviceData(
                    6, // deviceId
                    18.2f, // temperature
                    6.2f, // humidity
                    14.2f, // radon
                    20.1f, // PPM
                    10.5f, // airQuality
                    "300", // roomName
                    475342 // timeStamp
                ),
                new DeviceData(
                    7, // deviceId
                    18.2f, // temperature
                    6.2f, // humidity
                    14.2f, // radon
                    20.1f, // PPM
                    10.5f, // airQuality
                    "300", // roomName
                    475342 // timeStamp
                )
            ); // Initial dummy data
            dummyDevices.AddRange(
                new Device {
                    DeviceId = 0,
                    IsActive = true,
                    DeviceName = "SonicWave BT-500",
                    Firmware = "v1.0.1",
                    MACAddress = "1f:2b:60:6e:e5"
                },
                new Device
                {
                    DeviceId = 1,
                    IsActive = true,
                    DeviceName = "PulseLink Pro",
                    Firmware = "v1.0.1",
                    MACAddress = "f3:6e:69:6d:e5"
                },
                new Device
                {
                    DeviceId = 2,
                    IsActive = false,
                    DeviceName = "EchoBeam X2",
                    Firmware = "v1.0.1",
                    MACAddress = "1a:2c:e0:6e:e5"
                },
                new Device
                {
                    DeviceId = 3,
                    IsActive = false,
                    DeviceName = "NexusConnect Elite",
                    Firmware = "v1.0.1",
                    MACAddress = "1f:fb:cd:ce:e5"
                },
                new Device
                {
                    DeviceId = 4,
                    IsActive = false,
                    DeviceName = "WaveSphere Mini",
                    Firmware = "v1.0.1",
                    MACAddress = "0f:1f:69:9e:e5"
                },
                new Device
                {
                    DeviceId = 5,
                    IsActive = true,
                    DeviceName = "QuantumSync V3",
                    Firmware = "v1.0.1",
                    MACAddress = "ff:5b:e0:ee:e9"
                }
                ); // Initial dummy devices

            dummyData.Add(new DeviceData(
                    1, // deviceId
                    27.2f, // temperature
                    6.2f, // humidity
                    14.2f, // radon
                    20.1f, // PPM
                    10.5f, // airQuality
                    "300", // roomName
                    475768 // timeStamp
                )); // Adding extra dummy data

            app.MapGet("/GetDataForDevice", (HttpContext httpContext) =>
            {
                int? deviceId = ValidateId(httpContext);
                string roomName = httpContext.Request.Query["roomName"].ToString() ?? "N/A";

                var newList = new List<DeviceData>();

                Debug.WriteLine($"Recieved request - Device ID: {deviceId}, Room: {roomName}");

                foreach (var item in dummyData)
                {
                    if(deviceId == item.DeviceId)
                    {
                        newList.Add(item);
                    }
                }

                return Results.Ok(newList);
            });

            app.MapGet("/GetAllDevices", () =>
            {
                return Results.Ok(dummyDevices);
            });

            app.MapGet("/GetDevice", (HttpContext httpContext) =>
            {
                int? deviceId = ValidateId(httpContext);

                foreach (var item in dummyDevices)
                {
                    if (item.DeviceId.Equals(deviceId))
                    {
                        return Results.Ok(item);
                    }
                }
                return Results.NotFound("No devices with provided device ID");
            });

            app.MapPost("/PostDataForDevice", (HttpContext httpContext) =>
            {
                return Results.Ok("yay!");
            }); // TODO

            app.MapPost("/VerifyDevice", (HttpContext httpContext) =>
            {
                return Results.Ok("yay!");
            }); // TODO
                
            app.MapPost("/AddDevice", (Device device) =>
            {
                if (!IsFullObject(device))
                {
                    return Results.BadRequest(new
                    {
                        message = "A full device object is needed."
                    });
                }

                Debug.WriteLine($"ID: {device.DeviceId} - Name: {device.DeviceName} - Active: {device.IsActive}");

                return Results.Ok("The device has been added!");
            }); // TODO

            app.Run();
        }

        /// <summary>
        /// Will Validate and convert httpContext ID param to integer type. Provide httpContext that has deviceId as a param.
        /// Will return -1 if invalid value is provided or other type of variable is provided.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static int ValidateId(HttpContext httpContext)
        {
            int? deviceId = null;
            string deviceIdStr = httpContext.Request.Query["deviceId"];

            if (!string.IsNullOrEmpty(deviceIdStr))
            {
                if (int.TryParse(deviceIdStr, out int parsedDeviceId))
                {
                    deviceId = parsedDeviceId < 0 ? -1 : parsedDeviceId;
                }
                else
                {
                    deviceId = -1;
                }
            }
            else
            {
                deviceId = -1;
            }
            return (int)deviceId;
        }

        public static bool IsFullObject(Device device)
        {
            if (device.DeviceId == null
                || string.IsNullOrWhiteSpace(device.DeviceName)
                || string.IsNullOrWhiteSpace(device.Firmware)
                || string.IsNullOrWhiteSpace(device.MACAddress))
                return false;
            return true;
        }
    }
}
