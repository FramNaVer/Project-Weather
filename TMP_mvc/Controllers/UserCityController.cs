using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMP_mvc.Data;
using TMP_mvc.Models;
using TMP_mvc.ViewModels;

namespace TMP_mvc.Controllers
{
    [Authorize]
    public class UserCityController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserCityController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var vm = new UserCityViewModel
            {
                AllCities = await _context.Cities.ToListAsync(),
                FollowedCities = await _context.UserCities
             .Where(uc => uc.UserId == userId)
             .Include(uc => uc.City)
             .ToListAsync()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Follow(string cityName)
        {
            var userId = _userManager.GetUserId(User);
            var city = await _context.Cities
                .FirstOrDefaultAsync(c => c.Name.ToLower() == cityName.ToLower());

            if (city == null)
            {
                TempData["Error"] = $"ไม่พบเมืองชื่อ '{cityName}'";
                return RedirectToAction("Index");
            }

            var exists = await _context.UserCities
                .AnyAsync(uc => uc.UserId == userId && uc.CityId == city.Id);

            if (!exists)
            {
                _context.UserCities.Add(new UserCity { UserId = userId, CityId = city.Id });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> SearchCities(string term)
        {
            var results = await _context.Cities
                .Where(c => c.Name.Contains(term))
                .OrderBy(c => c.Name)
                .Select(c => new { label = c.Name, value = c.Name })
                .Take(10)
                .ToListAsync();

            return Json(results);
        }

        [HttpPost]
        public async Task<IActionResult> Unfollow(int cityId)
        {
            var userId = _userManager.GetUserId(User);
            var entity = await _context.UserCities.FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CityId == cityId);
            if (entity != null)
            {
                _context.UserCities.Remove(entity);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Stats()
        {
            var cityStats = await _context.UserCities
                .Include(uc => uc.City)
                .GroupBy(uc => new { uc.CityId, uc.City.Name })
                .Select(g => new CityStatsViewModel
                {
                    CityId = g.Key.CityId,
                    CityName = g.Key.Name,
                    FollowerCount = g.Count()
                })
                .OrderByDescending(x => x.FollowerCount)
                .Take(5)
                .ToListAsync();

            return View(cityStats);
        }

    }
}
