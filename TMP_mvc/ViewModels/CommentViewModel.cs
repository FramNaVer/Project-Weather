using System.ComponentModel.DataAnnotations;

namespace TMP_mvc.ViewModels
{
    public class CommentViewModel
    {
        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = "";
    }
}
