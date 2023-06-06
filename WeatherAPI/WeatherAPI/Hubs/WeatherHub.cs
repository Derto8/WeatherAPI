using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using WeatherAPI.Interfaces;
using WeatherSendClient;
using WeatherModels;
using System.Collections.Generic;
using WeatherAPI.Repository;
using WeatherAPI.DataBaseContext;
using Microsoft.Extensions.ObjectPool;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace WeatherAPI.Hubs
{
    public class WeatherHub : Hub<INotificationClient>
    {
        private readonly IConfiguration Configuration;
        private Uri apiUrl;
        private WeatherContext context;
        private WeatherRepository weatherRepository;
        public WeatherHub(IConfiguration configuration, WeatherContext context)
        {
            Configuration = configuration;
            this.context = context;
        }

        [Authorize]
        public async Task WeatherMethod(string city, string lattitude, string longitude)
        {
            weatherRepository = new WeatherRepository(context);
            if (await weatherRepository.FindCity(city))
            {
                List<Weather> weathers = await GetWeatherFromDB(city);
                await SendWeatherClient(weathers);
            }
            else
            {
                await AddWeather(city, lattitude, longitude);
            }
        }

        public async Task SendWeatherClient(List<Weather> weathers)
        {
            await Clients.Caller.Send(weathers);
        }

        public async Task AddWeather(string city, string lattitude, string longitude)
        {
            List<Weather> weatherSendClient = await GetWeather(city, lattitude, longitude);
            foreach(Weather weather in weatherSendClient)
            {
                await weatherRepository.Create(weather);
            }
            await SendWeatherClient(weatherSendClient);
        }

        public async Task<List<Weather>> GetWeatherFromDB(string city)
        {
            List<Weather> weathers = await weatherRepository.Get(city);
            return weathers;
        }


        public async Task<List<Weather>> GetWeather(string city, string lattitude, string longitude)
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
            List<Weather> weatherSendClient = new List<Weather>();

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

                    weather.City = city;
                    weather.Lattitude = lattitude;
                    weather.Longitude = longitude;
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
                    weatherSendClient.Add(weather);
                }
            }
            return weatherSendClient;
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
