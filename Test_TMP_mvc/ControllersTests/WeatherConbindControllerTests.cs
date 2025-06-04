using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using TMP_mvc.Controllers;
using TMP_mvc.Data;
using TMP_mvc.Hubs;
using TMP_mvc.Interfaces;
using TMP_mvc.Models;
using TMP_mvc.ViewModels;

public class WeatherConbindControllerTests
{
    [Fact]
    public async Task GetCityWeather_ReturnsJsonResult_WhenCityExists()
    {
        // Arrange: Mock IWeatherConbindService
        var mockWeatherService = new Mock<IWeatherConbindService>();
        mockWeatherService.Setup(s => s.GetWeatherByCityAsync("Bangkok"))
            .ReturnsAsync(new WeatherCombindViewModel { City = "Bangkok", Temp = 33 });

        mockWeatherService.Setup(s => s.GetWeatherCurrentlyAsync("Bangkok"))
            .ReturnsAsync(new { forecast = "sunny" });

        // Mock UserManager<ApplicationUser>
        var userStore = new Mock<IUserStore<ApplicationUser>>();
        var mockUserManager = new Mock<UserManager<ApplicationUser>>(
            userStore.Object, null, null, null, null, null, null, null, null);

        // Mock IHubContext<NotificationHub>
        var mockHub = new Mock<IHubContext<NotificationHub>>();

        // Use In-Memory Database for DbContext
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        using var context = new ApplicationDbContext(options);

        // Seed a matching city
        context.Cities.Add(new City { Name = "Bangkok", NameTh = "กรุงเทพมหานคร" });
        context.SaveChanges();

        // Create the controller with all dependencies injected
        var controller = new WeatherConbindController(
            mockWeatherService.Object,
            mockUserManager.Object,
            mockHub.Object,
            context
        );

        // Act: Call the method
        var result = await controller.GetCityWeather("กรุงเทพมหานคร");

        // Assert: Expect JsonResult
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.NotNull(jsonResult.Value);
    }
}
