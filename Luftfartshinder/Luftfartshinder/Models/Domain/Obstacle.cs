using System.ComponentModel.DataAnnotations;

namespace Luftfartshinder.Models.Domain
{
    public class Obstacle
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public int ReportId { get; set; }
        public Report Report { get; set; }

        /// <summary>
        /// Type of obstacle (e.g., "Powerline", "Mast", "Line", "Point", "Luftspenn").
        /// </summary>
        [Required]
        public string Type { get; set; }

        [Required(ErrorMessage = "Obstacle name is required.")]
        [MaxLength(100, ErrorMessage = "Obstacle name cannot exceed 40 characters.")]
        public string Name { get; set; }


        /// <summary>
        /// Height of the obstacle in meters. Maximum value is 200 meters.
        /// </summary>
        [Range(0, 200, ErrorMessage = "Height must be between 0 and 200 meters.")]        
        public double? Height { get; set;  }

        [Required(ErrorMessage = "Obstacle latitude is required.")]
        public double Latitude { get; set; }
        
        [Required(ErrorMessage = "Obstacle longitude is required.")]
        public double Longitude { get; set; }

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        /// <summary>
        /// Note added by registrar when reviewing this obstacle.
        /// </summary>
        public string? RegistrarNote { get; set; }
        
        /// <summary>
        /// Status of the obstacle review process.
        /// </summary>
        public enum Statuses 
        { 
            /// <summary>Obstacle is pending review</summary>
            Pending = 0, 
            /// <summary>Obstacle has been approved</summary>
            Approved = 1, 
            /// <summary>Obstacle has been rejected</summary>
            Rejected = 2 
        }
        
        public Statuses Status { get; set; } = Statuses.Pending;
    }
}
