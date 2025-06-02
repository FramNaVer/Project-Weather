using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TMP_mvc.Interfaces;

namespace TMP_mvc.Controllers
{
    public class WeatherApiController : Controller
    {

        private readonly IWeatherApiService _weatherService;

        public WeatherApiController(IWeatherApiService weatherService)
        {
            _weatherService = weatherService;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetWeatherByCity(string city)
        {
            try
            {
                var model = await _weatherService.GetWeatherByCityAsync(city);

                if (model == null)
                {
                    ModelState.AddModelError("", "ไม่พบข้อมูลเมืองที่ค้นหา");
                    return View("Index");
                }

                return View("Index", model);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("", "ไม่สามารถเชื่อมต่อกับบริการพยากรณ์อากาศได้");
            }
            catch (JsonException)
            {
                ModelState.AddModelError("", "ข้อมูลที่ได้รับไม่ถูกต้อง");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "เกิดข้อผิดพลาด: " + ex.Message);
            }

            return View("Index");
        }
    }
}
