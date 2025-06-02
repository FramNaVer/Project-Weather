namespace TMP_mvc.Interfaces
{
    public interface IForecastWeatherServices
    {
        Task<object> GetCurrentWeatherAsync(double lat, double lon);
        Task<object> GetForecastWeatherAsync(double lat, double lon);
    }
}
