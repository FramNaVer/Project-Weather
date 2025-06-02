namespace TMP_mvc.ViewModels
{
    public class WeatherViewModel
    {
        public string? City { get; set; }
        public double Temp { get; set; }
        public string? Description { get; set; }
        public string? IconUrl { get; set; }
        public int? Humidity { get; set; }
        public double? WindSpeed { get; set; }
        public int? Cloudiness { get; set; }

    }
}
