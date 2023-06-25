using Microsoft.EntityFrameworkCore;
using WeatherModels;
using WeatherSendClient;

namespace WeatherAPI.DataBaseContext
{
    public class WeatherContext : DbContext
    {
        public WeatherContext(DbContextOptions<WeatherContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<Weather> WeatherTable { get; set; }
        public DbSet<UserData> UsersData { get; set; }

    }
}
