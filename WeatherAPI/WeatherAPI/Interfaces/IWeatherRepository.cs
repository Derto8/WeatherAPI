using WeatherModels;

namespace WeatherAPI.Interfaces
{
    public interface IWeatherRepository
    {
        Task Create(Weather entity);
        Task<List<Weather>> Get(string city);
        Task Update(Weather weatherDataOld, Weather weatherDataNew);
        Task<bool> FindCity(string city);
        Task<List<Weather>> GetAllWeathers();
    }
}
