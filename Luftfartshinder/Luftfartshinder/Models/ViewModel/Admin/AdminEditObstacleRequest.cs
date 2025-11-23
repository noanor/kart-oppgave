using Luftfartshinder.Models.ViewModel.Obstacles;

namespace Luftfartshinder.Models.ViewModel.Admin

{
    public class AdminEditObstacleRequest : EditObstacleRequest
    {
        public string? RegistrarNote { get; set; }
    }
}
