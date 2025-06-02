namespace TMP_mvc.Models
{
    public class TemperatureHistory
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "";
        public ApplicationUser? User { get; set; }

        public int CityId { get; set; }
        public City? City { get; set; }

        public double Temperature { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
