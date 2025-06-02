using Newtonsoft.Json.Linq;
using TMP_mvc.ViewModels;

namespace TMP_mvc.Services
{
    public class AirQualityService : IAirQualityService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public AirQualityService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<AirQualityViewModel?> GetAirQualityAsync(double lat, double lon, string cityName = "ไม่ทราบชื่อเมือง")
        {
            string apiKey = _config["OpenWeather:ApiKey"];
            string airUrl = _config["OpenWeather:BaseUrlAir"];
            string reversUrl = _config["OpenWeather:BaseUrlReverse"];
            string reverseGeoUrl = $"{reversUrl}&lat={lat}&lon={lon}";

            // Step 1: ดึงข้อมูล Air Pollution
            var airResponse = await _httpClient.GetAsync($"{airUrl}?lat={lat}&lon={lon}&appid={apiKey}");
            if (!airResponse.IsSuccessStatusCode)
                return null;

            var airJson = JObject.Parse(await airResponse.Content.ReadAsStringAsync());
            var data = airJson["list"]?[0];

            // Step 2: หาก cityName ยังไม่รู้ ลอง reverse จาก lat/lon
            if (cityName == "ไม่ทราบชื่อเมือง")
            {
                try
                {
                    var geoRequest = new HttpRequestMessage(HttpMethod.Get, reverseGeoUrl);
                    geoRequest.Headers.Add("User-Agent", "air-quality-dashboard");

                    var geoResponse = await _httpClient.SendAsync(geoRequest);
                    if (geoResponse.IsSuccessStatusCode)
                    {
                        var geoJson = JObject.Parse(await geoResponse.Content.ReadAsStringAsync());
                        var address = geoJson["address"];
                        cityName = address?["city"]?.ToString()
                            ?? address?["town"]?.ToString()
                            ?? address?["village"]?.ToString()
                            ?? "ไม่ทราบชื่อเมือง";
                    }
                }
                catch
                {
                    cityName = "โหลดชื่อเมืองไม่สำเร็จ";
                }
            }

            // Step 3: รวมข้อมูลทั้งหมดเข้า ViewModel
            return new AirQualityViewModel
            {
                City = cityName,
                AQI = data?["main"]?["aqi"]?.ToObject<int>() ?? 0,
                PM2_5 = data?["components"]?["pm2_5"]?.ToObject<double>() ?? -1,
                PM10 = data?["components"]?["pm10"]?.ToObject<double>() ?? 0,
                CO = data?["components"]?["co"]?.ToObject<double>() ?? 0,
                NO2 = data?["components"]?["no2"]?.ToObject<double>() ?? 0,
                O3 = data?["components"]?["o3"]?.ToObject<double>() ?? 0,
                Lat = lat,
                Lon = lon
            };
        }

        public async Task<AirQualityViewModel?> GetAirQualityByCityNameAsync(string cityName)
        {
            string apiKey = _config["OpenWeather:ApiKey"];
            //string geoUrl = _config["OpenWeather:BaseUrlGeo"];
            string geoUrl = $"https://api.openweathermap.org/geo/1.0/direct?q={cityName}&limit=1&appid={apiKey}";

            var geoResponse = await _httpClient.GetAsync(geoUrl);
            if (!geoResponse.IsSuccessStatusCode) return null;

            var geoJson = await geoResponse.Content.ReadAsStringAsync();
            var geoData = JArray.Parse(geoJson);

            if (!geoData.Any()) return null;

            double lat = geoData[0]["lat"]?.ToObject<double>() ?? 0;
            double lon = geoData[0]["lon"]?.ToObject<double>() ?? 0;

            return await GetAirQualityAsync(lat, lon, cityName);
        }
    }
}
