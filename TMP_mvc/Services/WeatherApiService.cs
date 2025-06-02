using Newtonsoft.Json.Linq;
using TMP_mvc.Interfaces;
using TMP_mvc.ViewModels;

namespace TMP_mvc.Services
{
    public class WeatherApiService : IWeatherApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public WeatherApiService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<WeatherViewModel?> GetWeatherByCityAsync(string city)
        {
            try
            {
                string apiKey = _config["OpenWeather:ApiKey"];
                string baseUrl = _config["OpenWeather:BaseUrl"];
                string iconBase = _config["OpenWeather:IconBaseUrl"];
                string url = $"{baseUrl}?q={city}&units=metric&appid={apiKey}";

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return null;

                var json = JObject.Parse(await response.Content.ReadAsStringAsync());
                return new WeatherViewModel
                {
                    City = json["name"]?.ToString(),
                    Temp = json["main"]?["temp"]?.ToObject<double>() ?? 0,
                    Description = json["weather"]?[0]?["description"]?.ToString(),
                    IconUrl = $"{iconBase}{json["weather"]?[0]?["icon"]}@2x.png",
                    Humidity = json["main"]?["humidity"]?.ToObject<int>() ?? 0,
                    WindSpeed = json["wind"]?["speed"]?.ToObject<double>() ?? 0,
                    Cloudiness = json["clouds"]?["all"]?.ToObject<int>() ?? 0
                };
            }
            catch (Exception ex)
            {
                // Log หรือส่งข้อความ error กลับไป
                Console.WriteLine($"เกิดข้อผิดพลาด: {ex.Message}");
                return null;
            }
        }
    }
}
