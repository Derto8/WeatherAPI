using WeatherLibrary;

namespace WeatherAPI.Interfaces
{
    public interface INotificationClient
    {
        public Task Send(WeatherData data);
    }
}
