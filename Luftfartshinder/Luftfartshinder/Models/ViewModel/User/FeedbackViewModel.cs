using System.ComponentModel.DataAnnotations;

namespace Luftfartshinder.Models.ViewModel.User
{
    public class FeedbackViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Message is required")]
        [Display(Name = "Message")]
        [StringLength(2000, ErrorMessage = "Message cannot exceed 2000 characters")]
        public string Message { get; set; }

        [Display(Name = "Rating")]
        public int? Rating { get; set; }
    }

    public class QandAItem
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}

