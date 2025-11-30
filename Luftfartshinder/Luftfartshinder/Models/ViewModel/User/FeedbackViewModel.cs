using System.ComponentModel.DataAnnotations;

namespace Luftfartshinder.Models.ViewModel.User
{
    /// <summary>
    /// ViewModel for collecting user feedback.
    /// </summary>
    public class FeedbackViewModel
    {
        /// <summary>
        /// The name of the person providing feedback.
        /// </summary>
        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Email address of the person providing feedback.
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// Optional subject line for the feedback.
        /// </summary>
        [Display(Name = "Subject")]
        public string? Subject { get; set; }

        /// <summary>
        /// The feedback message content.
        /// </summary>
        [Required(ErrorMessage = "Message is required")]
        [Display(Name = "Message")]
        [StringLength(2000, ErrorMessage = "Message cannot exceed 2000 characters")]
        public string Message { get; set; }

        /// <summary>
        /// Optional rating value for the feedback.
        /// </summary>
        [Display(Name = "Rating")]
        public int? Rating { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the FeedbackViewModel with default values for Name, Email, and Message.
        /// </summary>
        public FeedbackViewModel()
        {
            Name = string.Empty;  
            Email = string.Empty;
            Message = string.Empty;
        }
    }

    /// <summary>
    /// Represents a question and answer item for FAQ display.
    /// </summary>
    public class QandAItem
    {
        /// <summary>
        /// The question text.
        /// </summary>
        public required string Question { get; set; }
        
        /// <summary>
        /// The answer text.
        /// </summary>
        public required string Answer { get; set; }
    }
}

