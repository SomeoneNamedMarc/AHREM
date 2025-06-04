using AHREM_API.Models;
using AHREM_API.Services;
using Microsoft.AspNetCore.Components.Sections;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using RTools_NTS.Util;
using StackExchange.Redis;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

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
            builder.Services.AddHttpClient();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            #endregion

            #region Users
            app.MapGet("/API/GetAllUsers", async (HttpContext httpContext, DBService dbService) =>
            {
                if (!IsValidToken(httpContext, key))
                {
                    return Results.Problem("Not logged in!");
                }

                var users = await dbService.GetAllUsersAsync();

                if (users != null && users.Count > 0)
                {
                    return Results.Ok(users);
                }
                return Results.NotFound("No users found in the database!");
            });

            app.MapGet("/API/GetUser", async (HttpContext httpContext, int? id, string? email, DBService dbService) =>
            {
                if (!IsValidToken(httpContext, key))
                {
                    return Results.Problem("Not logged in!");
                }

                if (id.HasValue)
                {
                    var user = await dbService.GetUserAsync(id.Value);
                    if (user != null)
                    {
                        return Results.Ok(user);
                    }
                    return Results.NotFound("No user with that ID found!");
                }
                else if (!string.IsNullOrEmpty(email))
                {
                    var user = await dbService.GetUserAsync(email);
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
            app.MapPost("/API/AddDevice", async (HttpContext httpContext, Device device, int verificationCode, DBService dbService) =>
            {
                ConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync(builder.Configuration["ConnectionStrings:Redis"]);
                IDatabase redisDB = redis.GetDatabase();
                var server = redis.GetServer(builder.Configuration["ConnectionStrings:Redis"]);

                //JsonCommands json = redisDB.JSON();

                if (!IsValidToken(httpContext, key))
                {
                    return Results.Problem("Not logged in!");
                }

                var allKeys = server.Keys(pattern: "verificationCode:*").ToArray();

                foreach (var key in allKeys)
                {
                    var jsonResult = await redisDB.JSON().GetAsync(key, ".");
                    if (!jsonResult.IsNull)
                    {
                        var verificationRequest = JsonSerializer.Deserialize<VerificationRequest>(jsonResult.ToString());
                        if (verificationRequest != null && verificationRequest.VerificationCode == verificationCode && verificationRequest.ID == device.ID)
                        {
                            // Verification successful, remove the key from Redis
                            await redisDB.KeyDeleteAsync(key);
                            await dbService.AddDeviceAsync(device);

                            return Results.Ok($"Verification successful for device ID: {device.ID}");
                        }
                    }
                }
                return Results.Problem("Verification failed! Please check the verification code and device ID.");
            });

            // Verifies device, temp code on display
            app.MapPost("/API/VerifyDevice", async (VerificationRequest request) =>
            {
                var verificationRequest = new VerificationRequest(request.VerificationCode, request.ID);

                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(builder.Configuration["ConnectionStrings:Redis"]);
                IDatabase redisDB = redis.GetDatabase();

                JsonCommands json = redisDB.JSON();

                string recordID = Guid.NewGuid().ToString("N");

                json.SetAsync($"verificationCode:{recordID}", "$", JsonSerializer.Serialize(verificationRequest)).Wait();
                await redisDB.KeyExpireAsync($"verificationCode:{recordID}", TimeSpan.FromSeconds(30));

                return Results.Ok("Code posted to redis");
            });

            // Removes device with given ID.
            app.MapGet("/API/RemoveDevice", async (HttpContext httpContext, int? id, DBService dbService) =>
            {
                if (!IsValidToken(httpContext, key))
                {
                    return Results.Problem("Not logged in!");
                }

                if (id != null)
                {
                    var result = await dbService.DeleteDeviceAsync(id.Value);
                    return Results.Ok(result);
                }
                return Results.BadRequest("No device with given ID!");
            });

            // Get information about a certain device.
            app.MapGet("/API/GetDevice", async (HttpContext httpContext, int? id, DBService dbService) =>
            {
                if (!IsValidToken(httpContext, key))
                {
                    return Results.Problem("Not logged in!");
                }

                if (id.HasValue)
                {
                    var test = await dbService.GetDeviceAsync(id.Value);

                    if (test != null)
                    {
                        return Results.Ok(test);
                    }
                    return Results.NotFound("No device with provided ID found!");
                }

                return Results.BadRequest("No device ID provided!");
            });

            // Get a list of all devices (for admins).
            app.MapGet("/API/GetAllDevices", async (HttpContext httpContext, DBService dbService) =>
            {
                if (!IsValidToken(httpContext, key))
                {
                    return Results.Problem("Not logged in!");
                }

                var devices = await dbService.GetAllDevicesAsync();
                return Results.Ok(devices);
            });
            #endregion

            #region Device Data
            // Get all data for device with a certain ID or room name.
            app.MapGet("/API/GetDataForDevice", async (HttpContext httpContext, int? id, string? roomName, DBService dbService) =>
            {
                if (!IsValidToken(httpContext, key))
                {
                    return Results.Problem("Not logged in!");
                }

                Debug.WriteLine($"Received request - Device ID: {id}, Room: {roomName}");

                if (id != null)
                {
                    var data = await dbService.GetDeviceDataForDeviceIdAsync(id.Value);
                    return Results.Ok(data);
                }
                else if (roomName != null)
                {
                    var data = await dbService.GetDeviceDataForRoomNameAsync(roomName);
                    return Results.Ok(data);
                }

                return Results.BadRequest("No device with that ID!");
            });

            // Post a device's measurement of air quality and all relevant data.
            app.MapPost("/API/PostDataForDevice", async (HttpContext httpContext, DeviceData deviceData, DBService dbService) =>
            {
                if (!IsValidToken(httpContext, key))
                {
                    return Results.Problem("Not logged in!");
                }

                var test = await dbService.PostDeviceDataAsync(deviceData);

                if (!test)
                {
                    return Results.Problem("Could not post data to DB!");
                }

                return Results.Ok("Posted successfully to DB!");
            });
            #endregion

            #region Login/Verify
            app.MapPost("/API/Login", async (HttpContext httpContext, LoginRequest loginRequest, DBService dbService) =>
            {
                ConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync(builder.Configuration["ConnectionStrings:Redis"]);
                IDatabase redisDB = redis.GetDatabase();

                JsonCommands json = redisDB.JSON();

                #region redis dummy data setup
                await json.SetAsync("test@gmail.com", "$", "\"test123\"");
                await redisDB.KeyExpireAsync("test@gmail.com", TimeSpan.FromDays(3));

                await json.SetAsync("MURK@mail.com", "$", "\"MURKINGIT!\"");
                await redisDB.KeyExpireAsync("test@gmail.com", TimeSpan.FromDays(3));

                await json.SetAsync("Jannick@mail.com", "$", "\"cwossdwessingUWU\"");
                await redisDB.KeyExpireAsync("test@gmail.com", TimeSpan.FromDays(3));
                #endregion

                var result = (string?)await json.GetAsync(key: loginRequest.Email);

                if (!string.IsNullOrEmpty(result))
                {
                    string trimmedPwd = result.Trim('"');

                    Debug.WriteLine($"loginRequest pwd: {loginRequest.Password}");
                    Debug.WriteLine($"result pwd: {result}");

                    if (loginRequest.Password.Equals(trimmedPwd))
                    {
                        var token = GenerateToken(loginRequest, key);
                        return Results.Ok(new { token });
                    }
                    return Results.Problem("Email or Password is incorrect!");
                }

                if (await dbService.CanLoginAsync(loginRequest) && key != null)
                {
                    var token = GenerateToken(loginRequest, key);

                    await json.SetAsync(loginRequest.Email, "$", $"\"{loginRequest.Password}\"");
                    await redisDB.KeyExpireAsync(loginRequest.Email, TimeSpan.FromDays(3));

                    return Results.Ok(new { token });
                }
                return Results.Problem("Login failed!");
            });
            #endregion

            app.Run();
        }

        public static bool IsValidToken(HttpContext httpContext, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }
            var authHeader = httpContext.Request.Headers.Authorization.FirstOrDefault();

            if (authHeader == null || !authHeader.StartsWith("Bearer "))
            {
                return false;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            try
            {
                var handler = new JsonWebTokenHandler();
                var tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

                var result = handler.ValidateTokenAsync(token, new TokenValidationParameters
                {
                    IssuerSigningKey = tokenKey,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true
                });

                if (!result.Result.IsValid)
                {
                    return false;
                }
                return result.Result.IsValid;
            }
            catch
            {
                return false;
            }
        }

        public static string GenerateToken(LoginRequest loginRequest, string key)
        {
            var handler = new JsonWebTokenHandler();
            var tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var token = handler.CreateToken(new SecurityTokenDescriptor
            {
                Claims = new Dictionary<string, object> { ["email"] = loginRequest.Email },
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha256)
            });
            return token;
        }
    }
    public record VerificationRequest(int VerificationCode, int ID);
}
