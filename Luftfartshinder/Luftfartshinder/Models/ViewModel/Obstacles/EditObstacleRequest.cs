using Luftfartshinder.Models.Domain;
using System.ComponentModel.DataAnnotations;

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
        public Report? Report { get; set; }
        
        /// <summary>Type of obstacle (e.g., "Powerline", "Mast", "Line", "Point", "Luftspenn").</summary>
        [Required(ErrorMessage = "Obstacle type is required.")]
        [StringLength(50, ErrorMessage = "Type cannot exceed 50 characters.")]
        public required string Type { get; set; }
        
        /// <summary>Name of the obstacle.</summary>
        [Required(ErrorMessage = "Obstacle name is required.")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public required string Name { get; set; }

        /// <summary>Height of the obstacle in meters.</summary>
        [Range(0, 200, ErrorMessage = "Height must be between 0 and 200 meters.")]
        public double? Height { get; set; }
        
        /// <summary>Latitude coordinate of the obstacle.</summary>
        [Required(ErrorMessage = "Latitude is required.")]
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90 degrees.")]
        public double Latitude { get; set; }
        
        /// <summary>Longitude coordinate of the obstacle.</summary>
        [Required(ErrorMessage = "Longitude is required.")]
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180 degrees.")]
        public double Longitude { get; set; }
        
        /// <summary>Description of the obstacle.</summary>
        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
        
        /// <summary>Current status of the obstacle review process.</summary>
        public Domain.Obstacle.Statuses Status { get; set; } = Domain.Obstacle.Statuses.Pending;
    }
}
