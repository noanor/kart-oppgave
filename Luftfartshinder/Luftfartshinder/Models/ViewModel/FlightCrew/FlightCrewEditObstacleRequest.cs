namespace Luftfartshinder.Models.ViewModel.FlightCrew
{
    /// <summary>
    /// ViewModel for editing obstacle details in the FlightCrew interface.
    /// FlightCrew cannot edit the Status field.
    /// </summary>
    public class FlightCrewEditObstacleRequest
    {
        /// <summary>Unique identifier for the obstacle.</summary>
        public int Id { get; set; }
        
        /// <summary>ID of the report this obstacle belongs to.</summary>
        public int ReportId { get; set; }
        
        /// <summary>Type of obstacle (e.g., "Powerline", "Mast", "Line", "Point", "Luftspenn").</summary>
        public required string Type { get; set; }
        
        /// <summary>Name of the obstacle.</summary>
        public required string Name { get; set; }

        /// <summary>Height of the obstacle in meters.</summary>
        public double? Height { get; set; }
        
        /// <summary>Latitude coordinate of the obstacle.</summary>
        public double Latitude { get; set; }
        
        /// <summary>Longitude coordinate of the obstacle.</summary>
        public double Longitude { get; set; }
        
        /// <summary>Description of the obstacle.</summary>
        public string? Description { get; set; }
    }
}


