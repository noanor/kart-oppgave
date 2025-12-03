using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Luftfartshinder.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {
        /// <summary>The first name of the user.</summary>
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "First name must be between 1 and 30 characters.")]
        public string FirstName { get; set; }
        
        /// <summary>The last name of the user.</summary>
        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "Last name must be between 1 and 30 characters.")]
        public string LastName { get; set; }
        
        /// <summary>Indicates whether the user has been approved by an administrator.</summary>
        public bool IsApproved { get; set; } = false;
        
        /// <summary>The ID of the organization the user belongs to.</summary>
        [Required(ErrorMessage = "Organization ID is required.")]
        public int OrganizationId { get; set; }
        
        /// <summary>The organization the user belongs to.</summary>
        public Organization Organization { get; set; }
    }
}