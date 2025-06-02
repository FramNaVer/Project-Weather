using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TMP_mvc.Services;


namespace TMP_mvc.Controllers
{
    public class AirQualityController : Controller
    {
        private readonly IAirQualityService _service;

        public AirQualityController(IAirQualityService service)
        {
            _service = service;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAirQualityByCity(string city)
        {
            var model = await _service.GetAirQualityByCityNameAsync(city);

            if (model == null)
                return BadRequest(new { message = "ไม่พบข้อมูลหรือชื่อเมืองไม่ถูกต้อง" });

            return Json(model);
        }

    }
}
