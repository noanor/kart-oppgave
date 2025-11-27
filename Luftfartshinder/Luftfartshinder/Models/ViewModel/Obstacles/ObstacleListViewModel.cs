namespace Luftfartshinder.Models.ViewModel.Obstacles
{
    /// <summary>
    /// ViewModel for displaying a list of obstacles with their details.
    /// </summary>
    public class ObstacleListViewModel
    {
        /// <summary>List of obstacle details to display.</summary>
        public List<EditObstacleRequest> ObstacleDetailsModels { get; set; } = new List<EditObstacleRequest>();
    }
}
