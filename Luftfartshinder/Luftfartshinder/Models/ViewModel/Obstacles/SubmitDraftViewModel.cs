using System.ComponentModel.DataAnnotations;

namespace Luftfartshinder.Models.ViewModel.Obstacles
{
    /// <summary>
    /// ViewModel for submitting a draft with report title and summary.
    /// </summary>
    public class SubmitDraftViewModel
    {
        /// <summary>
        /// Title of the report. Required field with maximum length of 100 characters.
        /// </summary>
        [Required(ErrorMessage = "Report title is required.")]
        [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public required string Title { get; set; }

        /// <summary>
        /// Summary of the report. Optional field with maximum length of 500 characters.
        /// </summary>
        [MaxLength(500, ErrorMessage = "Summary cannot exceed 500 characters.")]
        public string? Summary { get; set; }
    }
}


