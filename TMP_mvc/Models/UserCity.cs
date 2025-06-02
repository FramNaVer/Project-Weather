namespace TMP_mvc.Models
{
    public class UserCity
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "";
        public ApplicationUser? User { get; set; }

        public int CityId { get; set; }
        public City? City { get; set; }
    }
}
