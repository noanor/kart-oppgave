using System.ComponentModel.DataAnnotations;

namespace Luftfartshinder.Models.Domain
{
    public class Obstacle
    {
        // Unik ID for hver hindring
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public int ReportId { get; set; }
        public Report Report { get; set; }

        // Navn på hindringen – må fylles ut
        public string Type { get; set; } = "";

        [Required(ErrorMessage = "Obstacle name is required.")]
        public string Name { get; set; }

        // Intern lagring av høydeverdien
        private double? _height;

        // Høyde på hindringen – må være mellom 0 og 200
        //[Range(0, 200, ErrorMessage = "Obstacle height must be between 0 and 200.")]
        public double? Height
        {
            get => _height;
            set
            {
                // Sjekker at høyden ikke er over 200
                if (value > 200)
                {
                    throw new ArgumentOutOfRangeException(nameof(Height), "Obstacle height cannot exceed 200.");
                }
                _height = value;
            }
        }

        // Koordinater for hvor hindringen befinner seg – må fylles ut
        [Required(ErrorMessage = "Obstacle latitude is required.")]
        public double Latitude { get; set; }
        [Required(ErrorMessage = "Obstacle longitude is required.")]
        public double Longitude { get; set; }
        // Beskrivelse av hindringen – må fylles ut
        //[Required(ErrorMessage = "Obstacle description is required.")]
        public string? Description { get; set; }

        // Registrar ting
        public string? RegistrarNote { get; set; }
        public enum Statuses { Pending = 0, Approved = 1, Rejected = 2 }
        public Statuses Status { get; set; } = Statuses.Pending;
    }
}
