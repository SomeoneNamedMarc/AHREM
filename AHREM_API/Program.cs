
using AHREM_API.Models;
using AHREM_API.Services;
using Microsoft.AspNetCore.Components.Sections;
using MySqlConnector;
using System.Data;
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
            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.variables.json", optional: true)
                .AddEnvironmentVariables();
            builder.Services.AddAuthentication();

            builder.Services.AddScoped<DBService>();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            #region Users
            app.MapGet("GetAllUsers", (DBService dBService) =>
            {
                // TODO
            });

            app.MapGet("/GetUser", (int? id, string? email, DBService dbService) =>
            {
                if (id.HasValue)
                {
                }
            });

            app.Run();
            #endregion

            #region Devices
            // Adds new device to database.
            app.MapPost("/AddDevice", (Device device, DBService dBService) =>
            {
                var test = dBService.AddDevice(device);

                if (!test)
                {
                    return Results.Problem("Error while trying to add new device!");
                }

                return Results.Ok("The device has been added!");
            });

            // Removes device with given ID.
            app.MapGet("/RemoveDevice", (int? id, DBService dBService) =>
            {
                if (id != null)
                {
                    return Results.Ok(dBService.DeleteDevice(id.Value));
                }
                return Results.BadRequest("No device with given ID!");
            });

            // Get information about a certain device.
            app.MapGet("/GetDevice", async (int? id, DBService dbService) =>
            {
                var test = dbService.GetDevice(id.Value);

                if (test != null)
                {
                    return Results.Ok(test);
                }
                return Results.NotFound("No device with provided ID found!");
            });

            // Get a list of all device (for admins).
            app.MapGet("/GetAllDevices", (DBService dBService) =>
            {
                return Results.Ok(dBService.GetAllDevices);
            });
            #endregion

            #region Device Data
            // Get all data for device with a certain ID or room name.
            app.MapGet("/GetDataForDevice", (int? id, string? roomName, DBService dBService) =>
            {
                Debug.WriteLine($"Recieved request - Device ID: {id}, Room: {roomName}");

                if (id != null)
                {
                    return Results.Ok(dBService.GetDeviceDataForDeviceId(id.Value));
                }
                else if (roomName != null)
                {
                    return Results.Ok(dBService.GetDeviceDataForRoomName(roomName));
                }

                return Results.BadRequest("No device with that ID!");
            });

            // Post a devices measurment of airquality and all relevant data.
            app.MapPost("/PostDataForDevice", (DeviceData deviceData, DBService dBService) =>
            {
                var test = dBService.PostDeviceData(deviceData);

                if (!test)
                {
                    return Results.Problem("Could not post data to DB!");
                }

                return Results.Ok("Posted successfully to DB!");
            });

            app.MapPost("/VerifyDevice", (HttpContext httpContext) =>
            {
                return Results.Ok("yay!");
            }); // TODO
            #endregion
        }
    }
}
