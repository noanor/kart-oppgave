using Luftfartshinder.Models.Domain;
using Luftfartshinder.Repository;

namespace Luftfartshinder.Models.ViewModel.Organization
{
    public class OrgDataViewModel
    {
        public Domain.Organization Organization { get; set; }
        public List<Obstacle> Obstacles { get; set; } = new();
        public List<Report> Reports { get; set; } = new();
    }
}
