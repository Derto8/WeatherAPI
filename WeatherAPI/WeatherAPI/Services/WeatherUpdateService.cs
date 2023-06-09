﻿using Newtonsoft.Json.Linq;
using Quartz;
using System.Diagnostics;
using WeatherAPI.DataBaseContext;
using WeatherAPI.Hubs;
using WeatherAPI.Repository;
using WeatherModels;

namespace WeatherAPI.Services
{
    public class WeatherUpdateService : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IConfiguration Configuration;
        private Uri? apiUrl;
        private WeatherRepository weatherRepository;
        public WeatherUpdateService(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            Configuration = configuration;
            this.serviceScopeFactory = serviceScopeFactory;
        }
        //обновление погоды раз в час
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        WeatherContext? db = scope.ServiceProvider.GetRequiredService<WeatherContext>();
                        if (db != null)
                            weatherRepository = new WeatherRepository(db);
                        else
                        {
                            Debug.WriteLine("Не удалось получить базу данных");
                            await Task.CompletedTask;
                        }

                        List<Weather> weathers = await weatherRepository.GetAllWeathers();
                        List<Weather> weathersDistinctCity = weathers.DistinctBy(c => c.City).ToList();
                        List<string> cities = weathers.Select(c => c.City).Distinct().ToList();
                        List<string> lattitude = weathers.Select(c => c.Lattitude).Distinct().ToList();
                        List<string> longitude = weathers.Select(c => c.Longitude).Distinct().ToList();

                        List<List<Weather>> allWeathers = new List<List<Weather>>();


                        foreach (Weather weather in weathersDistinctCity)
                        {
                            allWeathers.Add(await GetWeather(weather.City, weather.Lattitude, weather.Longitude));
                        }
                        foreach (List<Weather> w in allWeathers)
                        {
                            if (w != null)
                            {
                                int i = 0;
                                foreach (List<Weather> listWeather in allWeathers)
                                {
                                    foreach (Weather weather in listWeather)
                                    {
                                        await weatherRepository.Update(weathers[i], weather);
                                        i++;
                                    }
                                }
                                Debug.WriteLine("Произошло обновление погоды в базе даных");
                                await Task.Delay(3600000);
                            }
                            else
                                continue;
                        }
                    }
                }
                catch(Exception ex)
                {
                    Debug.WriteLine($"Произошла ошибка: {ex.Data}\n{ex.Message}");
                    await Task.CompletedTask;
                }
            }
        }

        private async Task<List<Weather>> GetWeather(string city, string lattitude, string longitude)
        {
            int countSymbols = Configuration["UrlGetWeather"].IndexOf('&');
            string urlUpi = Configuration["UrlGetWeather"].Insert(countSymbols, lattitude);

            apiUrl = new Uri($"{urlUpi}{longitude}");
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-Yandex-API-Key", Configuration["YandexKey"]);
            HttpResponseMessage? resp = await httpClient.GetAsync(apiUrl);
            if (resp.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                for (int i = 0; i < 10; i++)
                {
                    Debug.WriteLine("Срок действия Yandex-токена закончился!");
                }
                return null;
            }
            List<Weather> weatherSendClient = new List<Weather>();
            if (resp.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string? json = await resp.Content.ReadAsStringAsync();
                JToken jObject = JObject.Parse(json);


                string[] dayCycle = new string[] { "morning", "day", "evening", "night" };

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
            }
            return weatherSendClient;
        }
    }
}
