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
            //Context.Entry(entity).State = EntityState.Added;
            Weather newWeather = entity;

            await DBSet.AddAsync(newWeather);
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

        public async Task<List<Weather>> GetAllWeathers()
        {
            List<Weather> weathers = await DBSet.ToListAsync();
            return weathers;
        }

        public async Task Update(Weather weatherDataOld, Weather weatherDataNew)
        {
            Context.Entry(weatherDataOld).State = EntityState.Modified;
            Weather? w = await DBSet.Where(c => c.Id == weatherDataOld.Id).FirstOrDefaultAsync();
            w.City = weatherDataNew.City;
            w.Lattitude = weatherDataNew.Lattitude;
            w.Longitude = weatherDataNew.Longitude;
            w.Date = weatherDataNew.Date;
            w.MaxTemperature = weatherDataNew.MaxTemperature;
            w.MinTemperature = weatherDataNew.MinTemperature;
            w.WeatherImageSource = weatherDataNew.WeatherImageSource;
            w.WeatherDescription = weatherDataNew.WeatherDescription;
            w.Pressure = weatherDataNew.Pressure;
            w.Humidity = weatherDataNew.Humidity;
            w.WindSpeed = weatherDataNew.WindSpeed;
            w.WindDir = weatherDataNew.WindDir;
            w.FeelsLike = weatherDataNew.FeelsLike;
            await Context.SaveChangesAsync();
        }

        public void Upd(Weather weatherDataOld, Weather weatherDataNew)
        {
            Weather? newWeather = DBSet.Where(c => c == weatherDataOld).FirstOrDefault();

            newWeather = weatherDataNew;
            Context.SaveChanges();
        }
    }
}
