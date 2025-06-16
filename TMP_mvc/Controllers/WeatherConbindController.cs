using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TMP_mvc.Data;
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
        private readonly ApplicationDbContext _context;

        public WeatherConbindController(IWeatherConbindService weatherService,
            UserManager<ApplicationUser> userManager,
            IHubContext<NotificationHub> hubContext,
            ApplicationDbContext context
            )
        {
            _weatherService = weatherService;
            _userManager = userManager;
            _hubContext = hubContext;
            _context = context;
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
                var matchedCity = await _context.Cities
                    .FirstOrDefaultAsync(c =>
                        c.Name.ToLower() == city.ToLower() ||
                        c.NameTh.ToLower() == city.ToLower());

                if (matchedCity == null)
                    return BadRequest(new { message = "ไม่พบข้อมูลเมือง หรือพยากรณ์อากาศ" });

                var current = await _weatherService.GetWeatherByCityAsync(matchedCity.Name);
                var forecast = await _weatherService.GetWeatherCurrentlyAsync(matchedCity.Name);

                if (current == null || forecast == null)
                    return BadRequest(new { message = "ไม่พบข้อมูลพยากรณ์อากาศ" });

                // ส่งชื่อที่จะแสดงไปด้วย
                current.City = matchedCity.NameTh ?? matchedCity.Name;

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
