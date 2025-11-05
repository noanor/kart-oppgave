using System.ComponentModel.DataAnnotations;

namespace Luftfartshinder.Models.ViewModel
{
    public class Obstacle
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Obstacle name is required.")]
        public string Name { get; set; }

        private int _height;

        [Range(0, 200, ErrorMessage = "Obstacle height must be between 0 and 200.")]
        public int Height
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

        [Required(ErrorMessage = "Obstacle description is required.")]
        public string Description { get; set; }


        [Required(ErrorMessage = "Obstacle latitude is required.")]
        public string Latitude { get; set; }
        [Required(ErrorMessage = "Obstacle longitude is required.")]
        public string Longitude { get; set; }

        public bool IsDraft { get; set; } = true;
        public string Type { get; set; } = "";
    }
}
