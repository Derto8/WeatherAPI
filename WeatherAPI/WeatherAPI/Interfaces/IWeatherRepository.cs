using WeatherModels;

namespace WeatherAPI.Interfaces
{
    public interface IWeatherRepository
    {
        void Create(Weather entity);
        List<Weather> Get();
        void Update(Weather item);
        Task<bool> FindCity(string city);
    }
}
