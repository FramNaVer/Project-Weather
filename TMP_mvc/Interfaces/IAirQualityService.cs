using TMP_mvc.ViewModels;

namespace TMP_mvc.Services
{
    public interface IAirQualityService
    {
        Task<AirQualityViewModel?> GetAirQualityByCityNameAsync(string cityName);

        //Dictionary<string, (double lat, double lon)> GetAvailableCities();
    }
}
