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
        public async void Create(Weather entity)
        {
            await DBSet.AddAsync(entity);
            await Context.SaveChangesAsync();
        }

        public async Task<bool> FindCity(string city)
        {
            Weather weather = await DBSet.Where(c => c.City == city).FirstOrDefaultAsync();
            if(weather != null)
                return true;
            return false;
        }

        public List<Weather> Get()
        {
            throw new NotImplementedException();
        }

        public async void Update(Weather item)
        {
            Context.Entry(item).State = EntityState.Modified;
            await Context.SaveChangesAsync();
        }
    }
}
