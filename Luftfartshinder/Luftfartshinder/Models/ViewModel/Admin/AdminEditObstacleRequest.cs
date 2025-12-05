using Luftfartshinder.Models.ViewModel.Obstacles;
using System.ComponentModel.DataAnnotations;

namespace Luftfartshinder.Models.ViewModel.Admin
{
    /// <summary>
    /// ViewModel for editing obstacles in the admin interface.
    /// Extends EditObstacleRequest to include registrar notes.
    /// </summary>
    public class AdminEditObstacleRequest : EditObstacleRequest
    {
        /// <summary>Optional note added by the registrar during review.</summary>
        [MaxLength(1000, ErrorMessage = "Registrar note cannot exceed 1000 characters.")]
        public string? RegistrarNote { get; set; }
    }
}
