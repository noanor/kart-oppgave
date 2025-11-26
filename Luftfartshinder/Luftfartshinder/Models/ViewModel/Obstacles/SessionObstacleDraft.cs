using Luftfartshinder.Models.Domain;
using System.Collections.Generic;

namespace Luftfartshinder.Models.ViewModel.Obstacles
{
    public class SessionObstacleDraft
    {
        public List<Domain.Obstacle> Obstacles { get; set; } = new();
        
        // Store line points for each obstacle by index
        // Key: obstacle index, Value: list of [lat, lng] coordinates
        public Dictionary<int, List<double[]>> LinePoints { get; set; } = new();
    }
}
