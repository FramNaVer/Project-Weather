namespace TMP_mvc.Models
{
    public class NotificationLog
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "";
        public ApplicationUser? User { get; set; }
        public string CityName { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
