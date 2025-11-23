using Luftfartshinder.Models.Domain;
using System.Collections.Generic;

namespace Luftfartshinder.Models.ViewModel.Obstacles
{
    public class SessionObstacleDraft
    {
        public List<Domain.Obstacle> Obstacles { get; set; } = new();
    }
}
