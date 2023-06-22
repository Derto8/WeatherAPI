using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;
using System.Text;
using WeatherModels;
using WebClient.Classies;
using WebClient.Models;

namespace WebClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IConfiguration Configuration;
        private readonly IWebHostEnvironment AppEnvironment;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, IWebHostEnvironment appEnvironment)
        {
            _logger = logger;
            Configuration = configuration;
            AppEnvironment = appEnvironment;
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

        //получает из ajax метода список погоды, преобразует в строку для удобного чтения и отсылает обратно
        [HttpPost]
        public IActionResult AjaxMethodGetWeather([FromBody] List<Weather> weathers)
        {
            string city = weathers[0].City;
            string weatherText = $"Weather forecast for the city: {city}\n";
            string[] dayMas = new string[] {"Morning:", "Day:", "Evening:", "Night:"};
            for(int i = 0; i < 28; i++)
            {
                string[] weatherDate = weathers[i].Date.ToString().Split(' ');
                weatherText += $"\nDate: {weatherDate[0]}\n";
                foreach (string day in dayMas)
                {
                    weatherText += $"\n{day}\n" +
                        $"Max temperature: {weathers[i].MaxTemperature}\n" +
                        $"Min temperature: {weathers[i].MinTemperature}\n" +
                        $"Weather description: {weathers[i].WeatherDescription}\n" +
                        $"Pressure: {weathers[i].Pressure}\n" +
                        $"Humidity: {weathers[i].Humidity}\n" +
                        $"WindDir: {weathers[i].WindDir}\n" +
                        $"Weather feels like: {weathers[i].FeelsLike}\n";
                }
                i += 3;
            }
           // return RedirectToAction("Method", "Home", new { weatherText = weatherText});
            return Json(weatherText);
        }

        [HttpGet]
        public IActionResult Method([FromQuery]string weatherText)
        {
            ViewData["Weather"] = weatherText;
            return Redirect("/Home/GetWeather");
            //var ms = new MemoryStream();
            //var writer = new StreamWriter(ms);
            //await writer.WriteAsync(weatherText);
            //await writer.FlushAsync();
            //ms.Position = 0;

            //Response.Headers.Add("Content-Disposition", "attachment;filename=some.txt");
            //return File(ms, "text/plain");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}