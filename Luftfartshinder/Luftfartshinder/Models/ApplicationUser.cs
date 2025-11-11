using Microsoft.AspNetCore.Identity;

namespace Luftfartshinder.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsApproved { get; set; } = false;
    }
}