using Microsoft.AspNetCore.Identity;

namespace Luftfartshinder.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsApproved { get; set; } = false;
        
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
    }
}