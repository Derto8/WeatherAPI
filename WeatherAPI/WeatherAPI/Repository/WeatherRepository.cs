using Microsoft.EntityFrameworkCore;
using WeatherAPI.Interfaces;
using WeatherModels;

namespace WeatherAPI.Repository
{
    public class WeatherRepository : IWeatherRepository
    {
        private DbContext Context;
        private DbSet<Weather> DBSet;
        public WeatherRepository(DbContext dbContext)
        {
            Context = dbContext;
            DBSet = Context.Set<Weather>();
        }
        public async Task Create(Weather entity)
        {
            await DBSet.AddAsync(entity);
            await Context.SaveChangesAsync();
        }

        public async Task<bool> FindCity(string city)
        {
            Weather? weather = await DBSet.Where(c => c.City.Equals(city)).FirstOrDefaultAsync();
            if(weather != null)
                return true;
            return false;
        }

        public async Task<List<Weather>> Get(string city)
        {
            List<Weather> weatherList = await DBSet.Where(c => c.City.Equals(city)).ToListAsync();
            return weatherList;
        }

        public async void Update(Weather item)
        {
            Context.Entry(item).State = EntityState.Modified;
            await Context.SaveChangesAsync();
        }
    }
}
