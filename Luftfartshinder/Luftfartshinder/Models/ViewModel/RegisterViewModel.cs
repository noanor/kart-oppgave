using System.ComponentModel.DataAnnotations;

namespace Luftfartshinder.Models.ViewModel
{
    public class RegisterViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email  { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        
        [Required(ErrorMessage = "Please select a role")]
        public string SelectedRole {get; set;}
    }
}
