using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Diagnostics;
using WeatherAPI.DataBaseContext;
using WeatherAPI.Hubs;
using WeatherAPI.Interfaces;
using WeatherAPI.Repository;
using WeatherAPI.Services;
using WeatherModels;

namespace WeatherAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSignalR();

            string? connection = builder.Configuration.GetConnectionString("ConnectionDataBase"); 
            builder.Services.AddDbContext<WeatherContext>(options => options.UseSqlServer(connection));

            builder.Services.AddHostedService<WeatherUpdateService>();


            var app = builder.Build();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<WeatherHub>("/weather");
            });

            app.Run();
        }
    }
}