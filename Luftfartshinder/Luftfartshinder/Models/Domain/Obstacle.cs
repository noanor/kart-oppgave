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
        public string Type { get; set; } = "";

        [Required(ErrorMessage = "Obstacle name is required.")]
        public string Name { get; set; }

        private double? _height;

        /// <summary>
        /// Height of the obstacle in meters. Maximum value is 200 meters.
        /// </summary>
        public double? Height
        {
            get => _height;
            set
            {
                if (value > 200)
                {
                    throw new ArgumentOutOfRangeException(nameof(Height), "Obstacle height cannot exceed 200.");
                }
                _height = value;
            }
        }

        [Required(ErrorMessage = "Obstacle latitude is required.")]
        public double Latitude { get; set; }
        
        [Required(ErrorMessage = "Obstacle longitude is required.")]
        public double Longitude { get; set; }

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
