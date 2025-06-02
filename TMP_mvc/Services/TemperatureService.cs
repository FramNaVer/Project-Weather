using Microsoft.Extensions.Localization;
using TMP_mvc.Interfaces;

namespace TMP_mvc.Services
{
    public class TemperatureService : ITemperatureService
    {
        private readonly IStringLocalizer<TemperatureService> _localizer;

        public TemperatureService(IStringLocalizer<TemperatureService> localizer)
        {
            _localizer = localizer;
        }

        public string GetWeatherDescription(double temp)
        {
            if (temp > 32) return _localizer["VeryHot"];
            else if (temp >= 25) return _localizer["Hot"];
            else if (temp >= 17) return _localizer["Warm"];
            else if (temp >= 11) return _localizer["Fresh"];
            else if (temp >= 0) return _localizer["Cold"];
            else return _localizer["VeryCold"];
        }

        public string GetIcon(double temp)
        {
            if (temp > 32) return "🔥";
            else if (temp >= 25) return "☀️";
            else if (temp >= 17) return "🌤️";
            else if (temp >= 11) return "🌥️";
            else if (temp >= 0) return "❄️";
            else return "🧊";
        }
    }
}
