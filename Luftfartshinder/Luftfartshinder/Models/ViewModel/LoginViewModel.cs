using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace Luftfartshinder.Models.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Brukernavn må fylles inn")]
        public string Brukernavn { get; set; }
        
        [Required(ErrorMessage = "Passord må fylles inn")]
        public string Passord { get; set; }
    }
}