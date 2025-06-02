using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using TMP_mvc.Data;
using TMP_mvc.Interfaces;
using TMP_mvc.Models;
using TMP_mvc.ViewModels;

namespace TMP_mvc.Services
{
    public class WeatherConbindService : IWeatherConbindService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;

        public WeatherConbindService(HttpClient httpClient, IConfiguration config, ApplicationDbContext context)
        {
            _httpClient = httpClient;
            _config = config;
            _context = context;
        }

        public async Task<WeatherCombindViewModel?> GetWeatherByCityAsync(string city)
        {
            string apiKey = _config["OpenWeather:ApiKey"];
            string baseUrl = _config["OpenWeather:BaseUrl"];
            string url = $"{baseUrl}?q={city}&units=metric&appid={apiKey}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = JObject.Parse(await response.Content.ReadAsStringAsync());

            return new WeatherCombindViewModel
            {
                City = json["name"]?.ToString(),
                Lat = json["coord"]?["lat"]?.ToObject<double>() ?? 0,
                Lon = json["coord"]?["lon"]?.ToObject<double>() ?? 0,
                Temp = json["main"]?["temp"]?.ToObject<double>() ?? 0,
                Description = json["weather"]?[0]?["description"]?.ToString(),
                IconUrl = $"https://openweathermap.org/img/wn/" +
                $"{json["weather"]?[0]?["icon"]}@2x.png"
            };
        }

        public async Task<object> GetWeatherCurrentlyAsync(string city)
        {
            string apiKey = _config["OpenWeather:ApiKey"];
            string baseUrl = _config["OpenWeather:BaseUrlForecast"];
            string url = $"{baseUrl}?q={city}&units=metric&appid={apiKey}";

            var response = await _httpClient.GetAsync(url);
            string result = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException("Failed to fetch forecast weather.");

            var json = JObject.Parse(result);

            var forecastList = json["list"]?.Take(5).Select(item => new
            {
                datetime = item["dt_txt"]?.ToString(),
                temp = item["main"]?["temp"]?.ToObject<double>(),
                description = item["weather"]?[0]?["description"]?.ToString(),
                icon = $"https://openweathermap.org/img/wn/" +
                $"{item["weather"]?[0]?["icon"]?.ToString()}@2x.png"
            });

            return new
            {
                city = json["city"]?["name"]?.ToString(),
                forecast = forecastList
            };
        }

        public async Task SaveTemperatureHistoryAsync(WeatherCombindViewModel vm, string userId)
        {
            var city = await _context.Cities.FirstOrDefaultAsync(c => c.Name == vm.City);
            if (city == null)
            {
                city = new City { Name = vm.City };
                _context.Cities.Add(city);
                await _context.SaveChangesAsync();
            }

            var history = new TemperatureHistory
            {
                UserId = userId,
                CityId = city.Id,
                Temperature = vm.Temp,
                Timestamp = DateTime.UtcNow
            };

            _context.TemperatureHistories.Add(history);
            await _context.SaveChangesAsync();
        }
    }
}
