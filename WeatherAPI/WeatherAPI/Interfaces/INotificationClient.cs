using WeatherModels;
using WeatherSendClient;
namespace WeatherAPI.Interfaces
{
    public interface INotificationClient
    {
        public Task Send(List<Weather> data);
    }
}
