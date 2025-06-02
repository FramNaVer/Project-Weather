namespace TMP_mvc.Models
{
    public class WeatherModel
    {
        public double Temperature { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
    }

    public class TempResultViewModel
    {
        public List<WeatherModel> Results { get; set; } = new();
    }

    public class TempInputViewModel
    {
        public double Temperature { get; set; }
        public string Unit { get; set; } = "C";
    }
}
