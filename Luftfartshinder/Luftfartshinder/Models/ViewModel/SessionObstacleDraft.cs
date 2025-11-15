using Luftfartshinder.Models.Domain;
using System.Collections.Generic;

namespace Luftfartshinder.Models.ViewModel
{
    public class SessionObstacleDraft
    {
        public List<Obstacle> Obstacles { get; set; } = new();
    }
}
