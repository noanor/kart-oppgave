namespace Luftfartshinder.Models.Domain
{
    public class Report
    {
        public Report() { }
        public int Id { get; set; }
        public string Author { get; set; }
        public string AuthorId { get; set; }
        public DateTime ReportDate { get; set; }
        public string Title { get; set; }

        public ICollection<Obstacle> Obstacles { get; set; } = [];

        public int GetTotalObstacles()
        {
            return Obstacles.Count;
        }
    }
}
