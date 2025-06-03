
using AHREM_API.Models;
using AHREM_API.Services;
using Microsoft.AspNetCore.Components.Sections;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using NRedisStack;
using NRedisStack.RedisStackCommands;
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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            #endregion

            #region Users
            app.MapGet("/API/GetAllUsers", (HttpContext httpContext, DBService dbService) =>
            {
                if (!IsValidToken(httpContext, key))
                {
                    return Results.Problem("Not logged in!");
                }

                var users = dbService.GetAllUsers();

                if (users != null && users.Count > 0)
                {
                    return Results.Ok(users);
                }
                return Results.NotFound("No users found in the database!");
            });

            app.MapGet("/API/GetUser", (HttpContext httpContext, int? id, string? email, DBService dbService) =>
            {
                if (!IsValidToken(httpContext, key))
                {
                    return Results.Problem("Not logged in!");
                }

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
            app.MapPost("/API/AddDevice", (HttpContext httpContext, Device device, DBService dbService) =>
            {
                if (!IsValidToken(httpContext, key))
                {
                    return Results.Problem("Not logged in!");
                }

                var test = dbService.AddDevice(device);

                if (!test)
                {
                    return Results.Problem("Error while trying to add new device!");
                }

                return Results.Ok("The device has been added!");
            });

            // Removes device with given ID.
            app.MapGet("/API/RemoveDevice", (HttpContext httpContext, int? id, DBService dbService) =>
            {
                if (!IsValidToken(httpContext, key))
                {
                    return Results.Problem("Not logged in!");
                }

                if (id != null)
                {
                    return Results.Ok(dbService.DeleteDevice(id.Value));
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

                var test = dbService.GetDevice(id.Value);

                if (test != null)
                {
                    return Results.Ok(test);
                }
                return Results.NotFound("No device with provided ID found!");
            });

            // Get a list of all device (for admins).
            app.MapGet("/API/GetAllDevices", (HttpContext httpContext, DBService dbService) =>
            {
                if (!IsValidToken(httpContext, key))
                {
                    return Results.Problem("Not logged in!");
                }

                return Results.Ok(dbService.GetAllDevices);
            });
            #endregion

            #region Device Data
            // Get all data for device with a certain ID or room name.
            app.MapGet("/API/GetDataForDevice", (HttpContext httpContext, int? id, string? roomName, DBService dbService) =>
            {
                if (!IsValidToken(httpContext, key))
                {
                    return Results.Problem("Not logged in!");
                }

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
            app.MapPost("/API/PostDataForDevice", (HttpContext httpContext, DeviceData deviceData, DBService dbService) =>
            {
                if (!IsValidToken(httpContext, key))
                {
                    return Results.Problem("Not logged in!");
                }

                var test = dbService.PostDeviceData(deviceData);

                if (!test)
                {
                    return Results.Problem("Could not post data to DB!");
                }

                return Results.Ok("Posted successfully to DB!");
            });

            app.MapPost("/API/VerifyDevice", (HttpContext httpContext) =>
            {
                return Results.Ok("yay!");
            }); // TODO
            #endregion

            #region Login/Verify
            app.MapPost("/API/Login", (HttpContext httpContext, LoginRequest loginRequest, DBService dbService) =>
            {
                Debug.WriteLine(builder.Configuration["ConnectionStrings:Redis"]);

                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(builder.Configuration["ConnectionStrings:Redis"]);
                IDatabase redisDB = redis.GetDatabase();

                JsonCommands json = redisDB.JSON();

                var userID = Guid.NewGuid().ToString("N");
                var key = $"User:{userID}";

                json.Set(key, "$", new
                {
                    Email = "test@mail.com",
                    Password = "test123"
                });

                userID = Guid.NewGuid().ToString("N");
                key = $"User:{userID}";

                json.Set(key, "$", new
                {
                    Email = "MURK@mail.com",
                    Password = "MURKINGIT!"
                });

                userID = Guid.NewGuid().ToString("N");
                key = $"User:{userID}";

                json.Set(key, "$", new
                {
                    Email = "Jannick@mail.com",
                    Password = "cwossdwessingUWU"
                });


                /*
                                User tempUser = new User();

                                foreach (var key in (string[])keys)
                                {
                                    var value = redisDB.StringGet(key);

                                    string token = string.Empty;
                                }

                                if (dbService.CanLogin(loginRequest) && key != null)
                                {
                                    string token = GenerateToken(loginRequest, key);

                                    var hash = new HashEntry[]
                                    {
                                        new HashEntry("email", loginRequest.Email),
                                        new HashEntry("password", loginRequest.Password)
                                    };

                                    redisDB.HashSet(loginRequest.Email, hash);
                                }
                */
                //return Results.Problem("Email or Password is incorrect!");

                return Results.Ok(json.Get(key: "User:"));
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
}
