using System.ComponentModel.DataAnnotations;

namespace Luftfartshinder.Models.Domain
{
    public class User
    {
        /// <summary>Unique identifier for the user.</summary>
        public Guid Id { get; set; }

        /// <summary>The username of the user.</summary>
        [Required(ErrorMessage = "Username is required.")]
        public required string UserName { get; set; }

        /// <summary>The email address of the user.</summary>
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public required string EmailAddress { get; set; }
        
        /// <summary>Indicates whether the user has been approved by an administrator.</summary>
        public bool IsApproved { get; set; }
        
        /// <summary>The role assigned to the user.</summary>
        [Required(ErrorMessage = "Role is required.")]
        public string Role { get; set; }
        
        /// <summary>The ID of the organization the user belongs to.</summary>
        [Required(ErrorMessage = "Organization ID is required.")]
        public int OrganizationId { get; set; }
        
        /// <summary>The organization the user belongs to.</summary>
        public Organization Organization { get; set; }
    }
}
