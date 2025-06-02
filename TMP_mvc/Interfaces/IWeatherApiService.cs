using TMP_mvc.ViewModels;

namespace TMP_mvc.Interfaces
{
    public interface IWeatherApiService
    {
        Task<WeatherViewModel?> GetWeatherByCityAsync(string city);
    }
}
