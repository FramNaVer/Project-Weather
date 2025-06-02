using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using TMP_mvc.Data;
using TMP_mvc.Hubs;
using TMP_mvc.Interfaces;

namespace TMP_mvc.Services
{
    public class WeatherForecastService : IForecastWeatherServices
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ApplicationDbContext _context;

        public WeatherForecastService(IConfiguration configuration,
            IHubContext<NotificationHub> hubContext,
            ApplicationDbContext context,
            HttpClient httpClient

            )
        {
            _httpClient = new HttpClient();
            _configuration = configuration;
            _hubContext = hubContext;
            _context = context;
        }

        public async Task<object> GetCurrentWeatherAsync(double lat, double lon)
        {
            string apiKey = _configuration["OpenWeather:ApiKey"];
            string baseUrl = _configuration["OpenWeather:BaseUrl"];
            string url = $"{baseUrl}?lat={lat}&lon={lon}&units=metric&appid={apiKey}";

            var response = await _httpClient.GetAsync(url);
            string result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException("Failed to fetch current weather.");

            var json = JObject.Parse(result);
            return new
            {
                city = json["name"]?.ToString(),
                temp = json["main"]?["temp"]?.ToObject<double>(),
                description = json["weather"]?[0]?["description"]?.ToString(),
                urlicon = $"https://openweathermap.org/img/wn/{json["weather"]?[0]?["icon"]?.ToString()}@2x.png"
            };
        }

        public async Task<object> GetForecastWeatherAsync(double lat, double lon)
        {
            string apiKey = _configuration["OpenWeather:ApiKey"];
            string baseUrl = _configuration["OpenWeather:BaseUrlForecast"];
            string url = $"{baseUrl}?lat={lat}&lon={lon}&units=metric&appid={apiKey}";

            var response = await _httpClient.GetAsync(url);
            string result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException("Failed to fetch forecast weather.");

            var json = JObject.Parse(result);

            // ดึง 5 แถวแรก (ข้อมูลทุก 3 ชม.)
            var forecastList = json["list"]?.Take(5).Select(item => new
            {
                datetime = item["dt_txt"]?.ToString(),
                temp = item["main"]?["temp"]?.ToObject<double>(),
                description = item["weather"]?[0]?["description"]?.ToString(),
                icon = $"https://openweathermap.org/img/wn/{item["weather"]?[0]?["icon"]?.ToString()}@2x.png"
            });

            return new
            {
                city = json["city"]?["name"]?.ToString(),
                forecast = forecastList
            };
        }
    }
}
