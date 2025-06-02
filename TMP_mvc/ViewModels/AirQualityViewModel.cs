namespace TMP_mvc.ViewModels
{
    public class AirQualityViewModel
    {
        public string City { get; set; } = "ไม่ทราบชื่อเมือง";
        public int AQI { get; set; }
        public double PM2_5 { get; set; }
        public double PM10 { get; set; }
        public double CO { get; set; }
        public double NO2 { get; set; }
        public double O3 { get; set; }

        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}
