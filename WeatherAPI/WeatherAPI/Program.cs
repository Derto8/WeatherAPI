using Microsoft.EntityFrameworkCore;
using WeatherAPI.DataBaseContext;
using WeatherAPI.Hubs;

namespace WeatherAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSignalR();

            string connection = builder.Configuration.GetConnectionString("ConnectionDataBase"); 
            builder.Services.AddDbContext<WeatherContext>(options => options.UseSqlServer(connection));

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