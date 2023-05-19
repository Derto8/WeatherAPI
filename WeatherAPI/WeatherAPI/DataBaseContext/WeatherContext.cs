using Microsoft.EntityFrameworkCore;
using WeatherModels;

namespace WeatherAPI.DataBaseContext
{
    public class WeatherContext : DbContext
    {
        public WeatherContext(DbContextOptions<WeatherContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        internal DbSet<Weather> WeatherTable { get; set; }
    }
}
