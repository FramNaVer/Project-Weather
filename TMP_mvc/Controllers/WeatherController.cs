using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TMP_mvc.Interfaces;
using TMP_mvc.Models;

namespace TMP_mvc.Controllers
{
    public class WeatherController : Controller
    {
        private readonly ITemperatureService _temService;
        public WeatherController(ITemperatureService temService)
        {
            _temService = temService;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string tempRaw, string unit)
        {
            var weatherResults = new List<WeatherModel>();

            try
            {
                // แปลงข้อความเป็น list ของ double
                var temperatures = tempRaw
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => double.TryParse(t.Trim(), out var val) ? val : double.NaN)
                    .Where(t => !double.IsNaN(t))
                    .ToList();

                foreach (var temp in temperatures)
                {
                    double tempCelsius = ConvertToCelsius(temp, unit);

                    if (tempCelsius < -100 || tempCelsius > 60)
                    {
                        ModelState.AddModelError("",
                        $"{temp} ({unit}): Out of acceptable range (-100 ถึง 60 °C)");
                        return View();
                    }

                    weatherResults.Add(new WeatherModel
                    {
                        Temperature = temp, // เก็บค่าที่ผู้ใช้กรอกจริง ๆ
                        Description = _temService.GetWeatherDescription(tempCelsius),
                        Icon = _temService.GetIcon(tempCelsius)
                    });
                }

                return View(new TempResultViewModel { Results = weatherResults });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "เกิดข้อผิดพลาดในการประมวลผล: " + ex.Message);
                return View();
            }
        }

        // แปลงอุณหภูมิเป็นเซลเซียส
        private double ConvertToCelsius(double temp, string unit)
        {
            return unit switch
            {
                "F" => (temp - 32) * 5 / 9,
                "K" => temp - 273.15,
                _ => temp // C ไม่ต้องแปลง
            };
        }
    }
}

