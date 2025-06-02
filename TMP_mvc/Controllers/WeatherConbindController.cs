using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TMP_mvc.Hubs;
using TMP_mvc.Interfaces;
using TMP_mvc.Models;

namespace TMP_mvc.Controllers
{
    [Authorize]
    public class WeatherConbindController : Controller
    {
        private readonly IWeatherConbindService _weatherService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<NotificationHub> _hubContext;

        public WeatherConbindController(IWeatherConbindService weatherService,
            UserManager<ApplicationUser> userManager,
            IHubContext<NotificationHub> hubContext
            )
        {
            _weatherService = weatherService;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCityWeather(string city)
        {
            try
            {
                var current = await _weatherService.GetWeatherByCityAsync(city);
                var forecast = await _weatherService.GetWeatherCurrentlyAsync(city);
                if (current == null || forecast == null)
                    return BadRequest(new { message = "ไม่พบข้อมูลเมือง หรือพยากรณ์อากาศ" });

                return Json(new
                {
                    current,
                    forecast
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "เกิดข้อผิดพลาด: " + ex.Message });
            }
        }

        public async Task<IActionResult> TriggerNotification()
        {
            string message = "🌡️ อุณหภูมิสูงเกิน 40°C ที่เชียงใหม่!";
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
            return Ok(new { message = "ส่งเรียบร้อย" });
        }
    }
}
