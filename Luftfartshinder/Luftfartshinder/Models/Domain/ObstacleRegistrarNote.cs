namespace Luftfartshinder.Models.Domain
{
    public class ObstacleRegistrarNote
    {
        public int Id { get; set; }
        public Guid RegistrarId { get; set; }
        public string Note { get; set; }
    }
}
