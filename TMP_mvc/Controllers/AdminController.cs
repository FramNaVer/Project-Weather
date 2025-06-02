using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMP_mvc.Data;
using TMP_mvc.Models;

namespace TMP_mvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var userRoles = new Dictionary<string, string>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = roles.FirstOrDefault() ?? "User";
            }

            ViewBag.UserRoles = userRoles;
            ViewBag.TotalUsers = users.Count;
            ViewBag.TotalAdmins = (await _userManager.GetUsersInRoleAsync("Admin")).Count;

            return View(users);
        }



        [HttpPost]
        public async Task<IActionResult> AssignAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null && !(await _userManager.IsInRoleAsync(user, "Admin")))
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                TempData["Message"] = $"เพิ่ม {user.Email} เป็น Admin เรียบร้อยแล้ว";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
                TempData["Message"] = $"ลบ {user.Email} จากสิทธิ์ Admin แล้ว";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ChartUser()
        {
            return View();
        }


        // Controller: AdminController.cs (เพิ่ม endpoint สำหรับข้อมูล Chart)
        [HttpGet]
        public async Task<IActionResult> GetUserChartData()
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            var allUsers = _userManager.Users.ToList();

            var result = new
            {
                AdminCount = admins.Count,
                UserCount = allUsers.Count - admins.Count
            };

            return Json(result);
        }


        [HttpGet]
        public IActionResult UserTimeline()
        {
            return View();
        }


        [HttpGet]
        public IActionResult GetUserTimelineData()
        {
            // ดึงข้อมูลจากฐานข้อมูลทั้งหมดมาก่อน
            var users = _userManager.Users
                .Select(u => new { u.CreatedAt }) // แค่เอา CreatedAt ก็พอ
                .ToList(); // ❗ เปลี่ยนเป็น in-memory ก่อนใช้ .ToString()

            // กลุ่มตามเดือนปี (ใน Memory)
            var grouped = users
                .GroupBy(u => u.CreatedAt.ToString("yyyy-MM"))
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    Month = g.Key,
                    Count = g.Count()
                }).ToList();

            return Json(grouped);
        }

        public async Task<IActionResult> AdminIndex()
        {
            var comments = await _context.Comments
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return View(comments);
        }

    }
}
