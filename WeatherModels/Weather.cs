namespace WeatherModels
{
    public class Weather
    {
        public DateTime? Date { get; set; }
        public string? MaxTemperature { get; set; }
        public string? MinTemperature { get; set; }
        public Uri? WeatherImageSource { get; set; }
        public string? WeatherDescription { get; set; }
        public string? Pressure { get; set; }
        public string? Humidity { get; set; }
        public string? WindSpeed { get; set; }
        public string? WindDir { get; set; }
        public string? FeelsLike { get; set; }
    }
}