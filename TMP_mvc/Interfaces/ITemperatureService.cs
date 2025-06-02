namespace TMP_mvc.Interfaces
{
    public interface ITemperatureService
    {
        string GetWeatherDescription(double temperature);
        string GetIcon(double temperature);
    }
}
