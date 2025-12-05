using Luftfartshinder.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace Luftfartshinder.Models.ViewModel.User
{
    /// <summary>
    /// ViewModel for editing and displaying report details in the registrar interface.
    /// </summary>
    public class EditReportRequest
    {
        /// <summary>Unique identifier for the report.</summary>
        public int Id { get; set; }
        
        /// <summary>Name of the author who created the report.</summary>
        public required string Author { get; set; }
        
        /// <summary>Unique identifier of the author user.</summary>
        public required string AuthorId { get; set; }
        
        /// <summary>Date when the report was created.</summary>
        public DateTime ReportDate { get; set; }
        
        /// <summary>Title of the report.</summary>
        [Required(ErrorMessage = "Report title is required.")]
        [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public required string Title { get; set; }
        
        /// <summary>Summary of the report.</summary>
        [MaxLength(500, ErrorMessage = "Summary cannot exceed 500 characters.")]
        public string? Summary { get; set; }

        /// <summary>Collection of obstacles associated with this report.</summary>
        public ICollection<Obstacle> Obstacles { get; set; } = [];
    }
}
