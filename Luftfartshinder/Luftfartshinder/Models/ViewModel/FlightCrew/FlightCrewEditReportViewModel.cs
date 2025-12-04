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
        public required string Title { get; set; }
        
        /// <summary>Summary of the report.</summary>
        public string? Summary { get; set; }
    }
}


