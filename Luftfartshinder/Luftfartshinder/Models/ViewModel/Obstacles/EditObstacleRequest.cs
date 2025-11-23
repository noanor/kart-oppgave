using Luftfartshinder.Models.Domain;

namespace Luftfartshinder.Models.ViewModel.Obstacles
{
    public class EditObstacleRequest
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public Report Report { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }

        private double? _height;
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
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Description { get; set; }
        public Domain.Obstacle.Statuses Status { get; set; } = Domain.Obstacle.Statuses.Pending;

    }
}
