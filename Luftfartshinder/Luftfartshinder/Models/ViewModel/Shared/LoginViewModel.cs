using System.ComponentModel.DataAnnotations;

namespace Luftfartshinder.Models.ViewModel.Shared
{
    /// <summary>
    /// ViewModel for user login authentication.
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>The username for authentication.</summary>
        [Required(ErrorMessage = "Username must be filled in")]
        public required string Username { get; set; }

        /// <summary>The password for authentication.</summary>
        [Required(ErrorMessage = "Password must be filled in")]
        public required string Password { get; set; }
    }
}
