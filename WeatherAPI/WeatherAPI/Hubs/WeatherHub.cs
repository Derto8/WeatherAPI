using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using WeatherAPI.Interfaces;
using WeatherSendClient;
using WeatherModels;
using System.Collections.Generic;
using WeatherAPI.DataClassies;

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


        public async Task SendWeatherMessage(string lattitude, string longitude)
        {
            int countSymbols = Configuration["UrlGetWeather"].IndexOf('&');
            string urlUpi = Configuration["UrlGetWeather"].Insert(countSymbols, lattitude);

            apiUrl = new Uri($"{urlUpi}{longitude}");
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-Yandex-API-Key", Configuration["YandexKey"]);
            HttpResponseMessage? resp = await httpClient.GetAsync(apiUrl);
            string? json = await resp.Content.ReadAsStringAsync();
            JToken jObject = JObject.Parse(json);


            string[] dayCycle = new string[] { "morning", "day", "evening", "night" };
            WeatherData weatherSendClient = new WeatherData();

            int daysTemp = jObject["forecasts"].Count();
            for (int i = 0; i < daysTemp; i++)
            {
                foreach (string c in dayCycle)
                {
                    Weather weather = new Weather();
                    DateTime date = jObject["forecasts"][i]["date"].ToObject<DateTime>();
                    string minTemp = jObject["forecasts"][i]["parts"][c]["temp_min"].ToObject<string>();
                    string maxTemp = jObject["forecasts"][i]["parts"][c]["temp_max"].ToObject<string>();
                    string pressure = jObject["forecasts"][i]["parts"][c]["pressure_mm"].ToObject<string>();
                    string humidity = jObject["forecasts"][i]["parts"][c]["humidity"].ToObject<string>();
                    string windSpeed = jObject["forecasts"][i]["parts"][c]["wind_speed"].ToObject<string>();
                    string windDir = jObject["forecasts"][i]["parts"][c]["wind_dir"].ToObject<string>();
                    string feelsLike = jObject["forecasts"][i]["parts"][c]["feels_like"].ToObject<string>();
                    string imageSource = jObject["forecasts"][i]["parts"][c]["icon"].ToObject<string>();
                    string weatherDesc = jObject["forecasts"][i]["parts"][c]["condition"].ToObject<string>();

                    weather.Date = date;
                    weather.MinTemperature = minTemp;
                    weather.MaxTemperature = maxTemp;
                    weather.Pressure = pressure;
                    weather.Humidity = humidity;
                    weather.WindSpeed = windSpeed;
                    weather.WindDir = windDir;
                    weather.FeelsLike = feelsLike;
                    weather.WeatherImageSource = new Uri($"https://yastatic.net/weather/i/icons/funky/dark/{imageSource}.svg");
                    weather.WeatherDescription = weatherDesc;
                    weatherSendClient.WeathersList.Add(weather);
                }
            }

            await Clients.Caller.Send(weatherSendClient.WeathersList);
        }

        public async Task ClientMessage(string message)
        {
            Debug.WriteLine(message);
        }
        protected override void Dispose(bool disposing)
        {
            Debug.WriteLine("Произошла очистка ресурсов");
        }
    }
}
