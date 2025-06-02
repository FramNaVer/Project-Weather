using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TMP_mvc.Data;
using TMP_mvc.Hubs;
using TMP_mvc.Interfaces;
using TMP_mvc.Models;
using TMP_mvc.Services;


DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient<WeatherForecastService>();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

//language support
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en-US"),
        new CultureInfo("th-TH")
    };

    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
});

builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

// 🗃️ Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 👤 Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// 🔐 Google OAuth
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    });

// 🍪 Cookie Policy for OAuth
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
    options.Secure = CookieSecurePolicy.Always;
});

// 🛠️ Identity Cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = false;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.HttpOnly = true;
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// 🌤️ Weather Services
builder.Services.AddHttpClient<IWeatherConbindService, WeatherConbindService>();
builder.Services.AddHttpClient<IWeatherApiService, WeatherApiService>();
builder.Services.AddScoped<ITemperatureService, TemperatureService>();
builder.Services.AddScoped<IForecastWeatherServices, WeatherForecastService>();
builder.Services.AddScoped<IAirQualityService, AirQualityService>();

//SignalR
builder.Services.AddSignalR();

var app = builder.Build();
app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
// ─────────────────────────────────────
// 🧩 Middleware pipeline
app.UseStaticFiles();
app.UseRouting();
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<NotificationHub>("/notifyHub");



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=WeatherConbind}/{action=Index}/{id?}");

app.MapRazorPages();


Console.WriteLine("Google ClientId: " + Environment.GetEnvironmentVariable("Authentication__Google__ClientId"));

app.Run();
