using Microsoft.AspNetCore.Mvc;
using Moq;
using TMP_mvc.Controllers;
using TMP_mvc.Interfaces;
using TMP_mvc.Models;

namespace Test_TMP_mvc.ControllersTests
{
    public class WeatherControllerTest
    {
        [Theory]
        [InlineData("C")]
        [InlineData("F")]
        [InlineData("K")]
        //input null test Unit
        public void Index_Post_WithEmptyTempRaw_ReturnsEmptyResults(string unit)
        {
            // Arrange
            var mockWeatherService = new Mock<ITemperatureService>();
            var controller = new WeatherController(mockWeatherService.Object);

            // Act
            var result = controller.Index("", unit);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<TempResultViewModel>(viewResult.Model);
            Assert.Empty(model.Results);
        }

        //Test celsius
        [Fact]
        public void Index_Post_ReturnsViewWithResult()
        {
            // Arr mock
            var mockWeatherService = new Mock<ITemperatureService>();
            mockWeatherService.Setup(s => s.GetWeatherDescription(It.IsAny<double>()))
                .Returns("Warm");
            mockWeatherService.Setup(s => s.GetIcon(It.IsAny<double>()))
                .Returns("🌤️");

            var controller = new WeatherController(mockWeatherService.Object);

            // Act: เรียก POST Index ด้วย tempRaw และ unit
            var result = controller.Index("30, 35", "C");

            // Assert: ตรวจว่าได้ ViewResult และมี model 
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<TempResultViewModel>(viewResult.Model);
            Assert.Equal(2, model.Results.Count);
            Assert.Equal("Warm", model.Results[0].Description);
            Assert.Equal("🌤️", model.Results[0].Icon);
        }

        //Test Fahrenheit
        [Fact]
        public void Index_Post_WithFahrenheitValues_ReturnsCorrectDescriptionsAndIcons()
        {
            // Arr mock
            var mockWeatherService = new Mock<ITemperatureService>();
            mockWeatherService.Setup(s => s.GetWeatherDescription(It.IsAny<double>()))
                .Returns("Cold");
            mockWeatherService.Setup(s => s.GetIcon(It.IsAny<double>()))
                .Returns("❄️");

            var controller = new WeatherController(mockWeatherService.Object);

            //Act Post Index ด้วย tempRaw และ unit
            var result = controller.Index("50, 45", "F");

            // Assert: ตรวจว่าได้ ViewResult และมี model 
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<TempResultViewModel>(viewResult.Model);
            Assert.Equal(2, model.Results.Count);
            Assert.Equal(50, model.Results[0].Temperature);
            Assert.All(model.Results, r =>
            {
                Assert.Equal("Cold", r.Description);
                Assert.Equal("❄️", r.Icon);
            });
        }

        //Test Kevin
        [Fact]
        public void Index_Post_KelvinToCelsius_ReturnsCorrectResult()
        {
            var mockService = new Mock<ITemperatureService>();
            mockService.Setup(s => s.GetWeatherDescription(It.IsAny<double>())).Returns("Warm");
            mockService.Setup(s => s.GetIcon(It.IsAny<double>())).Returns("🌤️");

            var controller = new WeatherController(mockService.Object);
            var result = controller.Index("300", "K");

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<TempResultViewModel>(viewResult.Model);
            Assert.Single(model.Results);
        }

        //The input data is invalid but the number is correct. 
        [Fact]
        public void Index_Post_PartialValidTemperature_ShouldIgnoreInvalid()
        {
            // Arr mock
            var mockWeatherService = new Mock<ITemperatureService>();
            var controller = new WeatherController(mockWeatherService.Object);

            // Act: เรียก POST Index ด้วย tempRaw ที่ไม่ถูกต้อง
            var result = controller.Index("invalid, 35", "C");

            // Assert: ตรวจว่าได้ ViewResult และมี ModelState error
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<TempResultViewModel>(viewResult.Model);
            Assert.Single(model.Results);
        }


        //Test input null , Invalid , string
        [Fact]
        public void Index_Post_AllInvalidTemperatures_ShouldReturnEmptyResults()
        {
            var mockService = new Mock<ITemperatureService>();
            var controller = new WeatherController(mockService.Object);

            var result = controller.Index("abc, --, null", "C");

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<TempResultViewModel>(viewResult.Model);
            Assert.Empty(model.Results);
        }

        //Test input over number
        [Fact]
        public void Index_Post_TemperatureOutOfRange_ReturnsModelError()
        {
            var mockService = new Mock<ITemperatureService>();
            var controller = new WeatherController(mockService.Object);

            var result = controller.Index("999", "C");

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.Contains("Out of acceptable range", controller.ModelState.Values.First().Errors.First().ErrorMessage);
        }

    }
}
