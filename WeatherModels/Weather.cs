namespace WeatherModels
{
    public class Weather
    {
        public int Id { get; set; }
        public string City { get; set; } = null!;
        public string Lattitude { get; set; } = null!;
        public string Longitude { get; set; } = null!;
        public DateTime Date { get; set; }
        public string MaxTemperature { get; set; } = null!;
        public string MinTemperature { get; set; } = null!;
        public Uri WeatherImageSource { get; set; } = null!;
        public string WeatherDescription { get; set; } = null!;
        public string Pressure { get; set; } = null!;
        public string Humidity { get; set; } = null!;
        public string WindSpeed { get; set; } = null!;
        public string WindDir { get; set; } = null!;
        public string FeelsLike { get; set; } = null!;
    }
}