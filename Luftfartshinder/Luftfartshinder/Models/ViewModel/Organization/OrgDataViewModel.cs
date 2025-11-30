using Luftfartshinder.Models.Domain;

namespace Luftfartshinder.Models.ViewModel.Organization
{
    /// <summary>
    /// ViewModel for displaying organization data including obstacles and reports.
    /// Used by FlightCrew to view their organization's obstacles and reports.
    /// </summary>
    public class OrgDataViewModel
    {
        /// <summary>The organization this data belongs to.</summary>
        public Domain.Organization Organization { get; set; } = null!;
        
        /// <summary>List of obstacles associated with this organization.</summary>
        public List<Obstacle> Obstacles { get; set; } = [];
        
        /// <summary>List of reports associated with this organization.</summary>
        public List<Report> Reports { get; set; } = [];
    }
}
