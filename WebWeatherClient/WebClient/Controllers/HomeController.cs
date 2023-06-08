using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using System.Diagnostics;
using WeatherModels;
using WebClient.Classies;
using WebClient.Models;

namespace WebClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IConfiguration Configuration;
        private HubConnection HubConnection;
        private Uri Url;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult AjaxMethodGetCity(string city)
        {
            if (city == null || string.IsNullOrWhiteSpace(city))
                return new BadRequestResult();
            string City = city.ToLower().UpperFirstChar();
            string? whatCity = CitiesClass.cities.Split("\r\n").Where(c => c.Contains(City)).FirstOrDefault();
            if(whatCity == null)
                return new BadRequestResult();
            string[] whatLatLon = whatCity.Split(' ');
            string lattitude = whatLatLon[2].Remove(whatLatLon[2].Length - 1);
            string longitude = whatLatLon[3];
            var data = new
            {
                сity = City,
                lattitude = lattitude,
                longitude = longitude,
            };
            return Json(data);
        }

        [HttpPost]
        public IActionResult AjaxMethodGetWeather(string a)
        {
            //foreach(Weather weather in listWeathers)
            //{
            //    Debug.WriteLine(weather.Id);
            //    Debug.WriteLine(weather.City);
            //    Debug.WriteLine(weather.Lattitude);
            //    Debug.WriteLine(weather.Longitude);
            //    Debug.WriteLine(weather.Date);
            //    Debug.WriteLine(weather.MaxTemperature);
            //    Debug.WriteLine(weather.MinTemperature);
            //    Debug.WriteLine(weather.WeatherImageSource);
            //    Debug.WriteLine(weather.WeatherDescription);
            //    Debug.WriteLine(weather.Pressure);
            //    Debug.WriteLine(weather.Humidity);
            //    Debug.WriteLine(weather.WindSpeed);
            //    Debug.WriteLine(weather.WindDir);
            //    Debug.WriteLine(weather.FeelsLike);
            //}
            return Json("");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}