using System.ComponentModel.DataAnnotations;

namespace Luftfartshinder.Models.ViewModel
{
    public class Obstacle
    {
        // Unik ID for hver hindring
        public int Id { get; set; }

        // Navn på hindringen – må fylles ut
        [Required(ErrorMessage = "Obstacle name is required.")]
        public string Name { get; set; }

        // Intern lagring av høydeverdien
        private int _height;

        // Høyde på hindringen – må være mellom 0 og 200
        [Range(0, 200, ErrorMessage = "Obstacle height must be between 0 and 200.")]
        public int Height
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

        // Beskrivelse av hindringen – må fylles ut
        [Required(ErrorMessage = "Obstacle description is required.")]
        public string Description { get; set; }

        // Koordinater for hvor hindringen befinner seg – må fylles ut
        [Required(ErrorMessage = "Obstacle latitude is required.")]
        public string Latitude { get; set; }
        [Required(ErrorMessage = "Obstacle longitude is required.")]
        public string Longitude { get; set; }

        public bool IsDraft { get; set; } = true;
        public string Type { get; set; } = "";
    }
}
