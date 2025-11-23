namespace Luftfartshinder.Models.Domain
{
    public class Organization
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }
        public ICollection<Report> Reports { get; set; }
    }
}
