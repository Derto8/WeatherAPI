using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherModels;

namespace WeatherAPIIntegrationTestig.TestExtensions
{
    internal static class DataDBContext
    {
        public static List<Weather> GetWeatherData()
        {
            List<Weather> weathers = new List<Weather>();
            DateTime dateTime = DateTime.Now;

            for (int i = 1; i <= 28; i++)
            {
                Random rnd = new Random();
                Weather weather = new Weather();
                weather.City = "Пермь";
                weather.Lattitude = "58.01";
                weather.Longitude = "56.25";
                weather.Date = dateTime.AddDays(i);
                weather.MinTemperature = rnd.Next(5, 15).ToString();
                weather.MaxTemperature = rnd.Next(16, 30).ToString();
                weather.Pressure = rnd.Next(720, 800).ToString();
                weather.Humidity = rnd.Next(70, 90).ToString();
                weather.WindSpeed = rnd.Next(3, 15).ToString();
                weather.WindDir = "North";
                weather.FeelsLike = rnd.Next(10, 20).ToString();
                weather.WeatherImageSource = new Uri($"https://yastatic.net/weather/i/icons/funky/dark/ova.ca.svg");
                weather.WeatherDescription = "Cloudy weather";
            }

            return weathers;
        }
    }
}
