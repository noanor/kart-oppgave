using Luftfartshinder.Models.Domain;

namespace Luftfartshinder.Models.ViewModel.User
{
    public class EditReportRequest
    {
        public EditReportRequest() { }
        public int Id { get; set; }
        public string Author { get; set; }
        public string AuthorId { get; set; }
        public DateTime ReportDate { get; set; }
        public string Title { get; set; }
        public string RegistrarNote { get; set; }

        public ICollection<Obstacle> Obstacles { get; set; } = [];
    }
}
