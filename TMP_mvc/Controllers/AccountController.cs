using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TMP_mvc.Models;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    // REGISTER
    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("📥 Register failed validation.");
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            CreatedAt = DateTime.UtcNow
        };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            _logger.LogInformation("✅ User registered successfully: {Email}", model.Email);
            return RedirectToAction("Login", "Account");
        }

        foreach (var error in result.Errors)
        {
            _logger.LogWarning("⚠️ Register error: {Error}", error.Description);
            ModelState.AddModelError("", error.Description);
        }

        return View(model);
    }

    // LOGIN
    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("📥 Login failed validation.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

        if (result.Succeeded)
        {
            _logger.LogInformation("✅ Login success for {Email}", model.Email);

            var user = await _userManager.FindByEmailAsync(model.Email);
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Admin"))
            {
                _logger.LogInformation("👑 Redirecting {Email} to Admin dashboard", model.Email);
                return RedirectToAction("Index", "Admin");
            }

            return RedirectToAction("Index", "WeatherConbind");
        }

        _logger.LogWarning("❌ Login failed for {Email}", model.Email);
        ModelState.AddModelError("", "Login failed");
        return View(model);
    }

    // LOGOUT
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("👋 User logged out.");
        return RedirectToAction("Login", "Account");
    }

    // GOOGLE LOGIN
    [HttpGet]
    public IActionResult ExternalLogin(string provider, string returnUrl = null)
    {
        _logger.LogInformation("🔐 Starting Google OAuth login...");
        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    [HttpGet]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
    {
        if (remoteError != null)
        {
            _logger.LogError("❌ Remote error from Google: {Error}", remoteError);
            ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
            return RedirectToAction(nameof(Login));
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            _logger.LogError("❌ Google login failed: GetExternalLoginInfoAsync returned null");
            return RedirectToAction(nameof(Login));
        }

        _logger.LogInformation("✅ External login info retrieved. Provider={Provider}, Key={Key}",
            info.LoginProvider, info.ProviderKey);

        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            var roles = await _userManager.GetRolesAsync(user);

            _logger.LogInformation("✅ External login success for {Email}", user.Email);

            if (roles.Contains("Admin"))
            {
                _logger.LogInformation("👑 Redirecting to Admin dashboard");
                return RedirectToAction("Index", "Admin");
            }

            return RedirectToAction("Index", "WeatherConbind");
        }

        // ❗ ยังไม่มีบัญชีผู้ใช้
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var userNew = new ApplicationUser { UserName = email, Email = email };

        var createResult = await _userManager.CreateAsync(userNew);
        if (createResult.Succeeded)
        {
            await _userManager.AddLoginAsync(userNew, info);
            await _signInManager.SignInAsync(userNew, isPersistent: false);

            _logger.LogInformation("🎉 Created new user via Google OAuth: {Email}", email);
            return RedirectToAction("Index", "WeatherConbind");
        }

        _logger.LogWarning("❌ Failed to create new user from Google OAuth.");
        return RedirectToAction(nameof(Login));
    }



    private IActionResult RedirectToLocal(string returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "WeatherConbind");
    }
}
