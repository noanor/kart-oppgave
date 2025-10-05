using System;
using System.ComponentModel.DataAnnotations;

namespace Oppgave1.Models
{
    public class ObstacleData
    {
        // Unik ID for hver hindring
        public int ObstacleID { get; set; }

        // Navn på hindringen – må fylles ut
        [Required(ErrorMessage = "Obstacle name is required.")]
        public string ObstacleName { get; set; }

        // Intern lagring av høydeverdien
        private int _obstacleHeight;

        // Høyde på hindringen – må være mellom 0 og 200
        [Range(0, 200, ErrorMessage = "Obstacle height must be between 0 and 200.")]
        public int ObstacleHeight
        {
            get => _obstacleHeight;
            set
            {
                // Sjekker at høyden ikke er over 200
                if (value > 200)
                {
                    throw new ArgumentOutOfRangeException(nameof(ObstacleHeight), "Obstacle height cannot exceed 200.");
                }
                _obstacleHeight = value;
            }
        }

        // Beskrivelse av hindringen – må fylles ut
        [Required(ErrorMessage = "Obstacle description is required.")]
        public string ObstacleDescription { get; set; }

        // Koordinater for hvor hindringen befinner seg – må fylles ut
        [Required(ErrorMessage = "Obstacle coordinates are required.")]
        public string ObstacleCoords { get; set; }
        
        public string GetLatitude()
        {
            return this.ObstacleCoords.Split(",")[0];
        }
        public string GetLongitude()
        {
            return this.ObstacleCoords.Split(",")[1];
        }
    }
}