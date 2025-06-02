using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TMP_mvc.Data;
using TMP_mvc.Models;
using TMP_mvc.ViewModels;

namespace TMP_mvc.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new CommentViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CommentViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Index", model);

            var userId = _userManager.GetUserId(User);
            var comment = new Comment
            {
                Message = model.Message,
                UserId = userId
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            ViewBag.SuccessMessage = "ส่งความคิดเห็นเรียบร้อยแล้ว ขอบคุณมาก!";
            return View("Index", new CommentViewModel());
        }
    }
}
