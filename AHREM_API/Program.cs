
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

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
                new Device(
                    0,
                    "SonicWave BT-500",
                    "v1.0.1",
                    "1f:2b:60:6e:e5"),
                new Device(
                    1,
                    "PulseLink Pro",
                    "v1.0.1",
                    "f3:6e:69:6d:e5"),
                new Device(
                    2,
                    "EchoBeam X2",
                    "v1.0.1",
                    "1a:2c:e0:6e:e5"),
                new Device(
                    3,
                    "NexusConnect Elite",
                    "v1.0.1",
                    "1f:fb:cd:ce:e5"), 
                new Device(
                    4,
                    "WaveSphere Mini",
                    "v1.0.1",
                    "0f:1f:69:9e:e5"),
                new Device(
                    5,
                    "QuantumSync V3",
                    "v1.0.1",
                    "ff:5b:e0:ee:e9")
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

            app.MapGet("/GetDevices", (HttpContext httpContext) =>
            {

                return Results.Ok("YIPPPIIIIIIEEEEEE!");
            });

            app.MapPost("/PostDataForDevice", (HttpContext httpContext) =>
            {
                return Results.Ok("yay!");
            }); // TODO

            app.MapPost("/VerifyDevice", (HttpContext httpContext) =>
            {
                return Results.Ok("yay!");
            }); // TODO

            app.MapPost("/AddDevice", (HttpContext httpContext) =>
            {
                return Results.Ok("yay!");
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
    }
}
