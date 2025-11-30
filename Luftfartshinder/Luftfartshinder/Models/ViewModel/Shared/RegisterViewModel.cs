using System.ComponentModel.DataAnnotations;

namespace Luftfartshinder.Models.ViewModel.Shared
{
    /// <summary>
    /// ViewModel for user registration by administrators.
    /// </summary>
    public class RegisterViewModel
    {
        /// <summary>The first name of the user being registered.</summary>
        [Required(ErrorMessage = "First name is required")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "First name must be between 1 and 30 characters")]
        public required string FirstName { get; set; }
        
        /// <summary>The last name of the user being registered.</summary>
        [Required(ErrorMessage = "Last name is required")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "Last name must be between 1 and 30 characters")]
        public required string LastName { get; set; }
        
        /// <summary>The email address of the user being registered.</summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public required string Email { get; set; }
        
        /// <summary>The username for the user being registered.</summary>
        [Required(ErrorMessage = "Username is required")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 30 characters")]
        public required string Username { get; set; }
        
        /// <summary>The password for the user being registered.</summary>
        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        public required string Password { get; set; }
        
        /// <summary>The role to assign to the user being registered.</summary>
        [Required(ErrorMessage = "Please select a role")]
        public required string SelectedRole { get; set; }
        
        /// <summary>Optional organization name if selecting an existing organization.</summary>
        public string? OrganizationName { get; set; }
        
        /// <summary>Optional name for creating a new organization.</summary>
        public string? OtherOrganizationName { get; set; }
    }
}
