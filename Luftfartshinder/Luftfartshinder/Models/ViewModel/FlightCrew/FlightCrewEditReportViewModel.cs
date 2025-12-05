using System.ComponentModel.DataAnnotations;

namespace Luftfartshinder.Models.ViewModel.FlightCrew
{
    /// <summary>
    /// ViewModel for editing report details in the FlightCrew interface.
    /// FlightCrew can only edit Title and Summary.
    /// </summary>
    public class FlightCrewEditReportViewModel
    {
        /// <summary>Unique identifier for the report.</summary>
        public int Id { get; set; }
        
        /// <summary>Title of the report.</summary>
        [Required(ErrorMessage = "Report title is required.")]
        [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public required string Title { get; set; }
        
        /// <summary>Summary of the report.</summary>
        [MaxLength(500, ErrorMessage = "Summary cannot exceed 500 characters.")]
        public string? Summary { get; set; }
    }
}


