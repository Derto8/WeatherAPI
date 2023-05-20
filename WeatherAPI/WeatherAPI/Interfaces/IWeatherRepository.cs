using WeatherModels;

namespace WeatherAPI.Interfaces
{
    public interface IWeatherRepository
    {
        Task Create(Weather entity);
        Task<List<Weather>> Get(string city);
        void Update(Weather item);
        Task<bool> FindCity(string city);
    }
}
