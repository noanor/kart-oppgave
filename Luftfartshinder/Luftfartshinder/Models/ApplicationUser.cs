using Microsoft.AspNetCore.Identity;

namespace Luftfartshinder.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Fornavn { get; set; }
        public string Etternavn { get; set; }
    }
}