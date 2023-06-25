using Azure.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WeatherAPI.Authorization;
using WeatherAPI.DataBaseContext;
using WeatherAPI.Hubs;
using WeatherAPI.Interfaces;
using WeatherAPI.Repository;
using WeatherAPI.Services;
using WeatherModels;
using WeatherSendClient;

namespace WeatherAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            string ISSUER = builder.Configuration["AuthOptions:ISSUER"];
            string AUDIENCE = builder.Configuration["AuthOptions:AUDIENCE"];
            string KEY = builder.Configuration["AuthOptions:KEY"];

            builder.Services.AddCors();
            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = ISSUER,
                        ValidateAudience = true,
                        ValidAudience = AUDIENCE,
                        ValidateLifetime = true,
                        IssuerSigningKey = AuthorizationExtensions.GetSymmetricSecurityKey(KEY),
                        ValidateIssuerSigningKey = true
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/weather"))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSignalR();

            string? connection = builder.Configuration.GetConnectionString("ConnectionDataBase"); 
            builder.Services.AddDbContext<WeatherContext>(options => options.UseSqlServer(connection));
            builder.Services.AddHostedService<WeatherUpdateService>();

            var app = builder.Build();

            app.UseCors(builder => builder.WithOrigins("https://localhost:7233").AllowAnyHeader().AllowAnyMethod().AllowCredentials());
            app.UseDefaultFiles();
            app.UseStaticFiles();
            //добавление мидлварей авторизации и аутентификации
            app.UseAuthentication();
            app.UseAuthorization();

            //пост запрос на авторизацию
            app.MapPost("/login", async (UserData userModel) =>
            {
                using(var scope = app.Services.CreateScope())
                {
                    WeatherContext? db = scope.ServiceProvider.GetService<WeatherContext>();
                    UserRepository userRepository;
                    if (db != null)
                        userRepository = new UserRepository(db);
                    else
                    {
                        Debug.WriteLine("Не удалось получить базу данных");
                        return Results.NoContent(); //ошибка с подключением к бд
                    }

                    UserData? user = await userRepository.FindUserAuth(userModel);
                    if (user is null) return Results.Unauthorized(); //ошибка 401

                    var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Login) };
                    var jwt = new JwtSecurityToken(
                        issuer: ISSUER,
                        audience: AUDIENCE,
                        claims: claims,
                        expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                        signingCredentials: new SigningCredentials(AuthorizationExtensions.GetSymmetricSecurityKey(KEY), SecurityAlgorithms.HmacSha256));
                    var encodedJWT = new JwtSecurityTokenHandler().WriteToken(jwt);

                    var response = new
                    {
                        access_token = encodedJWT,
                    };
                    return Results.Json(response);
                }
            });

            //пост запрос авторизации
            app.MapPost("/registration", async (UserData userModel) =>
            {
                using (var scope = app.Services.CreateScope())
                {
                    WeatherContext? db = scope.ServiceProvider.GetService<WeatherContext>();
                    UserRepository userRepository;
                    if (db != null)
                        userRepository = new UserRepository(db);
                    else
                    {
                        Debug.WriteLine("Не удалось получить базу данных");
                        return Results.NoContent(); //ошибка с подключением к бд
                    }

                    if (await userRepository.FindUserReg(userModel)) return Results.Conflict(); //ошибка 409, если данный пользователь уже существует

                    await userRepository.RegistrationUser(userModel); //если пользователь не найден, регистрируем его

                    var claims = new List<Claim> { new Claim(ClaimTypes.Name, userModel.Login) };
                    var jwt = new JwtSecurityToken(
                        issuer: ISSUER,
                        audience: AUDIENCE,
                        claims: claims,
                        expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                        signingCredentials: new SigningCredentials(AuthorizationExtensions.GetSymmetricSecurityKey(KEY), SecurityAlgorithms.HmacSha256));
                    var encodedJWT = new JwtSecurityTokenHandler().WriteToken(jwt);

                    var response = new
                    {
                        access_token = encodedJWT,
                    };
                    return Results.Json(response);
                }
            });

            app.MapHub<WeatherHub>("/weather");

            app.Run();
        }
    }
}