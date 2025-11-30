using Luftfartshinder.Models.ViewModel.Obstacles;

namespace Luftfartshinder.Models.ViewModel.Admin
{
    /// <summary>
    /// ViewModel for editing obstacles in the admin interface.
    /// Extends EditObstacleRequest to include registrar notes.
    /// </summary>
    public class AdminEditObstacleRequest : EditObstacleRequest
    {
        /// <summary>Optional note added by the registrar during review.</summary>
        public string? RegistrarNote { get; set; }
    }
}
