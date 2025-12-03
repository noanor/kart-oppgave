using System.ComponentModel.DataAnnotations;

namespace Luftfartshinder.Models.Domain
{
    public class Organization
    {
        /// <summary>Unique identifier for the organization.</summary>
        public int Id { get; set; }
        
        /// <summary>The name of the organization.</summary>
        [Required(ErrorMessage = "Organization name is required.")]
        public string Name { get; set; }
        
        /// <summary>Collection of users belonging to this organization.</summary>
        public ICollection<ApplicationUser> Users { get; set; } = [];
        
        /// <summary>Collection of reports submitted by this organization.</summary>
        public ICollection<Report> Reports { get; set; } = [];
    }
}
