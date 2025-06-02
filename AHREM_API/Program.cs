
using AHREM_API.Models;
using AHREM_API.Services;
using Microsoft.AspNetCore.Components.Sections;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using System.Data;
using System.Diagnostics;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace AHREM_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region Setup
            var builder = WebApplication.CreateBuilder(args);

            // Secret
            var key = builder.Configuration["Jwt:Key"];

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
            #endregion

            #region Users
            app.MapGet("GetAllUsers", (DBService dbService) =>
            {
                var users = dbService.GetAllUsers();
                if (users != null && users.Count > 0)
                {
                    return Results.Ok(users);
                }
                return Results.NotFound("No users found in the database!");
            });

            app.MapGet("/GetUser", (int? id, string? email, DBService dbService) =>
            {
                if (id.HasValue)
                {
                    var user = dbService.GetUser(id.Value);
                    if (user != null)
                    {
                        return Results.Ok(user);
                    }
                    return Results.NotFound("No user with that ID found!");
                }
                else if (!string.IsNullOrEmpty(email))
                {
                    var user = dbService.GetUser(email);
                    if (user != null)
                    {
                        return Results.Ok(user);
                    }
                    return Results.NotFound("No user with that email found!");
                }
                return Results.BadRequest("No user ID or email provided!");
            });
            #endregion

            #region Devices
            // Adds new device to database.
            app.MapPost("/AddDevice", (Device device, DBService dbService) =>
            {
                var test = dbService.AddDevice(device);

                if (!test)
                {
                    return Results.Problem("Error while trying to add new device!");
                }

                return Results.Ok("The device has been added!");
            });

            // Removes device with given ID.
            app.MapGet("/RemoveDevice", (int? id, DBService dbService) =>
            {
                if (id != null)
                {
                    return Results.Ok(dbService.DeleteDevice(id.Value));
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
            app.MapGet("/GetAllDevices", (DBService dbService) =>
            {
                return Results.Ok(dbService.GetAllDevices);
            });
            #endregion

            #region Device Data
            // Get all data for device with a certain ID or room name.
            app.MapGet("/GetDataForDevice", (int? id, string? roomName, DBService dbService) =>
            {
                Debug.WriteLine($"Recieved request - Device ID: {id}, Room: {roomName}");

                if (id != null)
                {
                    return Results.Ok(dbService.GetDeviceDataForDeviceId(id.Value));
                }
                else if (roomName != null)
                {
                    return Results.Ok(dbService.GetDeviceDataForRoomName(roomName));
                }

                return Results.BadRequest("No device with that ID!");
            });

            // Post a devices measurment of airquality and all relevant data.
            app.MapPost("/PostDataForDevice", (DeviceData deviceData, DBService dbService) =>
            {
                var test = dbService.PostDeviceData(deviceData);

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

            #region Login/Verify
            app.MapPost("/Login", (LoginRequest loginRequest, DBService dbService) =>
            {
                if (dbService.CanLogin(loginRequest) && key != null)
                {
                    var handler = new JsonWebTokenHandler();
                    var tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

                    var token = handler.CreateToken(new SecurityTokenDescriptor
                    {
                        Claims = new Dictionary<string, object>{ ["user"] = loginRequest.Username },
                        Expires = DateTime.UtcNow.AddHours(1),
                        SigningCredentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha256)
                    });
                    return Results.Ok(new { token });
                }
                return Results.Unauthorized();
            });
            #endregion

            app.Run();
        }
    }
}
