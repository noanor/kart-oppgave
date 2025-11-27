using Luftfartshinder.Models.Domain;

namespace Luftfartshinder.Models.ViewModel.Obstacles
{
    /// <summary>
    /// ViewModel for editing obstacle details in the draft view.
    /// </summary>
    public class EditObstacleRequest
    {
        /// <summary>Unique identifier for the obstacle.</summary>
        public int Id { get; set; }
        
        /// <summary>ID of the report this obstacle belongs to.</summary>
        public int ReportId { get; set; }
        
        /// <summary>The report entity this obstacle belongs to.</summary>
        public Report Report { get; set; }
        
        /// <summary>Type of obstacle (e.g., "Powerline", "Mast", "Line", "Point", "Luftspenn").</summary>
        public string Type { get; set; }
        
        /// <summary>Name of the obstacle.</summary>
        public string Name { get; set; }

        /// <summary>Height of the obstacle in meters.</summary>
        public double? Height { get; set; }
        
        /// <summary>Latitude coordinate of the obstacle.</summary>
        public double Latitude { get; set; }
        
        /// <summary>Longitude coordinate of the obstacle.</summary>
        public double Longitude { get; set; }
        
        /// <summary>Description of the obstacle.</summary>
        public string Description { get; set; }
        
        /// <summary>Current status of the obstacle review process.</summary>
        public Domain.Obstacle.Statuses Status { get; set; } = Domain.Obstacle.Statuses.Pending;
    }
}
