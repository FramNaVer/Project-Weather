using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TMP_mvc.Interfaces;

namespace TMP_mvc.Controllers
{
    public class WeatherForecastController : Controller
    {
        private readonly IForecastWeatherServices _weatherServices;

        public WeatherForecastController(IForecastWeatherServices weatherServices)
        {
            _weatherServices = weatherServices;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]

        public async Task<IActionResult> GetCurrent(double lat, double lon)
        {
            try
            {
                var data = await _weatherServices.GetCurrentWeatherAsync(lat, lon);
                return Json(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]

        public async Task<IActionResult> GetForecast(double lat, double lon)
        {
            try
            {
                var data = await _weatherServices.GetForecastWeatherAsync(lat, lon);
                return Json(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
