using System.ComponentModel.DataAnnotations;

namespace Luftfartshinder.Models.ViewModel
{
    public class RegisterViewModel
    {
        
        [Required(ErrorMessage = "First name is required")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "First name must be between 1 and 30 characters")]
        public string FirstName { get; set; }
        
        [Required(ErrorMessage = "Last name is required")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "Last name must be between 1 and 30 characters")]
        public string LastName { get; set; }
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email  { get; set; }
        
        [Required(ErrorMessage = "Username is required")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 30 characters")]
        public string Username { get; set; }
        
        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "Please select a role")]
        public string SelectedRole {get; set;}
        
        public string? Organization { get; set; }
        
        public string? OtherOrganization { get; set; }
    }
}
