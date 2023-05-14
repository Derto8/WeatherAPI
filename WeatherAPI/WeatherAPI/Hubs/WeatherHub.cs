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


            string[] dayCycle = new string[] { "morning", "day", "evening", "night" };
            WeatherData weatherSendClient = new WeatherData();

            for (int i = 0; i < dayCycle.Length; i++)
            {
                DateTime?[] date = jObject["forecasts"].Select(c => c["date"].ToObject<DateTime?>()).ToArray();
                string?[] minTemp = jObject["forecasts"].Select(c => c["parts"][dayCycle[i]]["temp_min"].ToObject<string?>()).ToArray();
                string?[] maxTemp = jObject["forecasts"].Select(c => c["parts"][dayCycle[i]]["temp_max"].ToObject<string?>()).ToArray();
                string?[] pressure = jObject["forecasts"].Select(c => c["parts"][dayCycle[i]]["pressure_mm"].ToObject<string?>()).ToArray();
                string?[] humidity = jObject["forecasts"].Select(c => c["parts"][dayCycle[i]]["humidity"].ToObject<string?>()).ToArray();
                string?[] windSpeed = jObject["forecasts"].Select(c => c["parts"][dayCycle[i]]["wind_speed"].ToObject<string?>()).ToArray();
                string?[] feelsLike = jObject["forecasts"].Select(c => c["parts"][dayCycle[i]]["feels_like"].ToObject<string?>()).ToArray();
                string?[] imageSource = jObject["forecasts"].Select(c => c["parts"][dayCycle[i]]["icon"].ToObject<string?>()).ToArray();
                string?[] weatherDesc = jObject["forecasts"].Select(c => c["parts"][dayCycle[i]]["condition"].ToObject<string?>()).ToArray();
                string?[] UVIndex = jObject["forecasts"].Select(c => c["parts"][dayCycle[i]]["uv_index"].ToObject<string?>()).ToArray();
                string?[] MagneticField = jObject["forecasts"].Select(c => c["biomet"]["condition"].ToObject<string?>()).ToArray();


                for (int j = 0; j < maxTemp.Length; j++)
                {
                    Weather weather = new Weather();
                    weather.Date = date[j];
                    weather.MinTemperature = minTemp[j];
                    weather.MaxTemperature = maxTemp[j];
                    weather.Pressure = pressure[j];
                    weather.Humidity = humidity[j];
                    weather.WindSpeed = windSpeed[j];
                    weather.FeelsLike = feelsLike[j];
                    weather.WeatherImageSource = $"https://yastatic.net/weather/i/icons/funky/dark/{imageSource[j]}.svg";
                    weather.WeatherDescription = weatherDesc[j];
                    weather.UVIndex = UVIndex[j];
                    weather.MagnecicField = MagneticField[j];

                    weatherSendClient.WeathersList.Add(weather);
                }
            }

            return Clients.Others.Send(weatherSendClient);
        }
        protected override void Dispose(bool disposing)
        {
            Debug.WriteLine("Произошла очистка ресурсов");
        }
    }
}
