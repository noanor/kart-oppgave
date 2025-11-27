using Luftfartshinder.Models.Domain;

namespace Luftfartshinder.Models.ViewModel.Obstacles
{
    /// <summary>
    /// ViewModel for storing draft obstacles in session before submission.
    /// </summary>
    public class ObstacleDraftViewModel
    {
        /// <summary>List of obstacles in the current draft session.</summary>
        public List<Domain.Obstacle> Obstacles { get; set; } = new();
    }
}
