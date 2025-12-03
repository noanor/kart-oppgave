using Luftfartshinder.Models.ViewModel.Shared;
using System.ComponentModel.DataAnnotations;

namespace Luftfartshinder.Models.ViewModel.User
{
    /// <summary>
    /// ViewModel for user registration with password confirmation.
    /// Extends RegisterViewModel to include password confirmation field.
    /// </summary>
    public class UserRegisterViewModel : RegisterViewModel
    {
        /// <summary>Confirmation of the password to ensure it matches the original password.</summary>
        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public required string ConfirmPassword { get; set; }
    }
}