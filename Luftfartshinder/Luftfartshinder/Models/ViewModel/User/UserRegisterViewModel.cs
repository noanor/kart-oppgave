using System.ComponentModel.DataAnnotations;

namespace Luftfartshinder.Models.ViewModel.User
{
    public class UserRegisterViewModel : RegisterViewModel
    {
        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}