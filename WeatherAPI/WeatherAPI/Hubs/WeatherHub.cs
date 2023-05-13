using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using WeatherAPI.Interfaces;
using WeatherSendClient;
using WeatherModels;

namespace WeatherAPI.Hubs
{
    public class WeatherHub : Hub<INotificationClient>
    {
        private readonly IConfiguration Configuration;
        private Uri apiUrl;
        public WeatherHub(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public async Task<Task> SendWeatherMessage()
        {
            apiUrl = new Uri(Configuration["UrlGetWeather"]);
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-Yandex-API-Key", Configuration["YandexKey"]);
            HttpResponseMessage? resp = await httpClient.GetAsync(apiUrl);
            string? json = await resp.Content.ReadAsStringAsync();
            JToken jObject = JObject.Parse(json);

            DateTime?[] date = jObject["forecasts"].Select(c => c["date"].ToObject<DateTime?>()).ToArray();
            string?[] minTemp = jObject["forecasts"].Select(c => c["parts"]["day"]["temp_min"].ToObject<string?>()).ToArray();
            string?[] maxTemp = jObject["forecasts"].Select(c => c["parts"]["day"]["temp_max"].ToObject<string?>()).ToArray();
            string?[] avgTemp = jObject["forecasts"].Select(c => c["parts"]["day"]["temp_avg"].ToObject<string?>()).ToArray();
            string?[] pressure = jObject["forecasts"].Select(c => c["parts"]["day"]["pressure_mm"].ToObject<string?>()).ToArray();
            string?[] humidity = jObject["forecasts"].Select(c => c["parts"]["day"]["humidity"].ToObject<string?>()).ToArray();
            string?[] windSpeed = jObject["forecasts"].Select(c => c["parts"]["day"]["wind_speed"].ToObject<string?>()).ToArray();
            string?[] feelsLike = jObject["forecasts"].Select(c => c["parts"]["day"]["feels_like"].ToObject<string?>()).ToArray();

            WeatherData weatherSendClient = new WeatherData();
            for (int i = 0; i < minTemp.Length; i++)
            {
                Weather weather = new Weather();
                weather.Date = date[i];
                weather.MinTemperature = minTemp[i];
                weather.MaxTemperature = maxTemp[i];
                weather.AVGTemperature = avgTemp[i];
                weather.Pressure = pressure[i];
                weather.Humidity = humidity[i];
                weather.WindSpeed = windSpeed[i];
                weather.FeelsLike = feelsLike[i];

                weatherSendClient.WeathersList.Add(weather);
            }

            return Clients.Others.Send(weatherSendClient);
        }
        protected override void Dispose(bool disposing)
        {
            Debug.WriteLine("Произошла очистка ресурсов");
        }
    }
}
