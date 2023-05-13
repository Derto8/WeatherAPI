namespace WeatherModels
{
    public class Weather
    {
        public DateTime? Date { get; set; }
        public string? MaxTemperature { get; set; }
        public string? MinTemperature { get; set; }
        public string? AVGTemperature { get; set; }
        public string? Pressure { get; set; }
        public string? Humidity { get; set; }
        public string? WindSpeed { get; set; }
        public string? FeelsLike { get; set; }
    }
}