using System.ComponentModel.DataAnnotations;

namespace Luftfartshinder.Models.Domain
{
    public class Report
    {
        public int Id { get; set; }
        
        public Organization Organization { get; set; }
        
        [Required(ErrorMessage = "Organization ID is required.")]
        public int OrganizationId { get; set; }
        
        [Required(ErrorMessage = "Author is required.")]
        public string Author { get; set; }
        
        [Required(ErrorMessage = "Author ID is required.")]
        public required string AuthorId { get; set; }
        
        [Required(ErrorMessage = "Report date is required.")]
        public DateTime ReportDate { get; set; }
        
        public required string Title { get; set; }
        
        /// <summary>
        /// Note added by registrar when reviewing this report.
        /// </summary>
        public string? RegistrarNote { get; set; }

        /// <summary>
        /// Collection of obstacles associated with this report.
        /// </summary>
        public ICollection<Obstacle> Obstacles { get; set; } = [];
    }
}
