namespace Luftfartshinder.Models.ViewModel
{
    public class Report
    {
        public Report() { }
        public int Id { get; set; }
        public string Author { get; set; } = "";
        public DateTime ReportDate { get; set; }
        public string Title { get; set; } = "";

        public ICollection<Obstacle> Obstacles { get; set; } = [];

        public int GetTotalObstacles()
        {
            return this.Obstacles.Count;
        }
    }
}
