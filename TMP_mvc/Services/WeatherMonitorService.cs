using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TMP_mvc.Data;
using TMP_mvc.Hubs;

namespace TMP_mvc.Services
{
    public class WeatherMonitorService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<NotificationHub> _hub;

        public WeatherMonitorService(IServiceScopeFactory scopeFactory, IHubContext<NotificationHub> hub)
        {
            _scopeFactory = scopeFactory;
            _hub = hub;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var users = await db.Users
                    .Include(u => u.UserCities)
                    .ThenInclude(uc => uc.City)
                    .ToListAsync();

                foreach (var user in users)
                {
                    foreach (var city in user.UserCities.Select(uc => uc.City))
                    {
                        var forecastDescriptions = await GetForecastDescriptionsAsync(city.Name);

                        foreach (var desc in forecastDescriptions)
                        {
                            if (desc.Contains("rain", StringComparison.OrdinalIgnoreCase))
                            {
                                string alertMessage = desc.ToLower() switch
                                {
                                    var d when d.Contains("light rain") => $"🌦️ ฝนตกเล็กน้อยที่ {city.NameTh ?? city.Name}",
                                    var d when d.Contains("moderate rain") => $"🌧️ ฝนตกปานกลางที่ {city.NameTh ?? city.Name}",
                                    var d when d.Contains("heavy") => $"⛈️ ฝนตกหนักที่ {city.NameTh ?? city.Name}",
                                    _ => $"🌧️ มีฝนตกที่ {city.NameTh ?? city.Name}"
                                };

                                await _hub.Clients.User(user.Id).SendAsync("ReceiveRainAlert", alertMessage);
                                break; // แจ้งเตือนแค่รอบเดียวพอ
                            }
                        }
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private async Task<List<string>> GetForecastDescriptionsAsync(string cityName)
        {
            await Task.Delay(100);
            return new List<string>
            {
                "clear sky",
                "moderate rain",
                "broken clouds"
            };
        }
    }
}
