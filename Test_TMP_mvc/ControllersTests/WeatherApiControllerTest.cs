using Microsoft.AspNetCore.Mvc;
using Moq;
using TMP_mvc.Controllers;
using TMP_mvc.Interfaces;
using TMP_mvc.ViewModels;

namespace Test_TMP_mvc.ControllersTests
{
    public class WeatherApiControllerTest
    {
        [Fact]
        public void GetWeatherByCityEN_ShouldReturnViewWithModel_WhenCityExists()
        {
            // Arrange
            var mockService = new Mock<IWeatherApiService>();
            mockService.Setup(s => s.GetWeatherByCityAsync("Bangkok"))
                .ReturnsAsync(new WeatherViewModel
                {
                    City = "Bangkok",
                    Temp = 30.5,
                    Description = "Sunny",
                    IconUrl = "http://example.com/icon.png",
                    Humidity = 70,
                    WindSpeed = 5.0,
                    Cloudiness = 20
                });
            var controller = new WeatherApiController(mockService.Object);
            // Act
            var result = controller.GetWeatherByCity("Bangkok").Result;
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<WeatherViewModel>(viewResult.Model);
            Assert.Equal("Bangkok", model.City);
        }

        [Fact]
        public void GetWeatherByCityTH_ShouldReturnViewWithModel_WhenCityExists()
        {
            // Arrange
            var mockService = new Mock<IWeatherApiService>();
            mockService.Setup(s => s.GetWeatherByCityAsync("เชียงใหม่"))
                .ReturnsAsync(new WeatherViewModel
                {
                    City = "เชียงใหม่",
                    Temp = 29.5,
                    Description = "ฝนตก",
                    IconUrl = "http://example.com/icon.png",
                    Humidity = 80,
                    WindSpeed = 6.0,
                    Cloudiness = 40
                });
            var controller = new WeatherApiController(mockService.Object);
            // Act
            var result = controller.GetWeatherByCity("เชียงใหม่").Result;
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<WeatherViewModel>(viewResult.Model);
            Assert.Equal("เชียงใหม่", model.City);
        }
    }
}
