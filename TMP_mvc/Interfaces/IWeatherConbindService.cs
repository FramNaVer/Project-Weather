using TMP_mvc.ViewModels;

namespace TMP_mvc.Interfaces
{
    public interface IWeatherConbindService
    {
        Task<WeatherCombindViewModel?> GetWeatherByCityAsync(string city);
        Task<object> GetWeatherCurrentlyAsync(string city);
    }
}
