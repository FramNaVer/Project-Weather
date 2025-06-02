namespace TMP_mvc.ViewModels
{
    public class CityModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public Coord Coord { get; set; }
    }

    public class Coord
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}
