namespace TMP_mvc.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public string Message { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string UserId { get; set; } = "";
        public ApplicationUser User { get; set; } = null!;
    }
}
