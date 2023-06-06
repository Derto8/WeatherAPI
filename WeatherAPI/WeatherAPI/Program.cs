using Azure.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
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
        //временна€ бд, позже создать таблицу
        static List<UserData> users = new List<UserData>
        {
            new UserData{ Login = "usr", Password="123"},
            new UserData{Login = "user", Password="1234"},
            new UserData{Login="", Password="1"},
            new UserData{Login="1", Password=""}
        };

        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ј¬“ќ–»«ј÷»я
            string ISSUER = builder.Configuration["AuthOptions:ISSUER"];
            string AUDIENCE = builder.Configuration["AuthOptions:AUDIENCE"];
            string KEY = builder.Configuration["AuthOptions:KEY"];

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

            //до реализации авторизации
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSignalR();

            string? connection = builder.Configuration.GetConnectionString("ConnectionDataBase"); 
            builder.Services.AddDbContext<WeatherContext>(options => options.UseSqlServer(connection));

            builder.Services.AddHostedService<WeatherUpdateService>();

            var app = builder.Build();
            //

            app.UseDefaultFiles();
            app.UseStaticFiles();
            //добавление мидлварей авторизации и аутентификации
            app.UseAuthentication();
            app.UseAuthorization();
            //
            app.MapPost("/login", (UserData userModel) =>
            {
                UserData? user = users.FirstOrDefault(p => p.Login == userModel.Login && p.Password == userModel.Password);
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
            });

            //до реализации авторизации
            //app.UseRouting(); //в случае ошибкок в хабе погоды раскомментить

            app.MapHub<WeatherHub>("/weather");

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapHub<WeatherHub>("/weather");
            //});

            app.Run();
        }

        //позже расширить данный класс и сделать таблицей бд
        //record class User(string Login, string Password);

        //class User
        //{
        //    public string Login { get; set; }
        //    public string Password { get; set; }
        //}
    }
}