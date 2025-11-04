using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Luftfartshinder.Models.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username must be filled in")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password must be filled in")]
        public string Password { get; set; }
    }
}
