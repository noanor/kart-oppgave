using System.ComponentModel.DataAnnotations;

namespace Luftfartshinder.Models
{
    public class User
    {
        public int Id { get; set; }
        [MaxLength(15), Required]
        public string UserName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [MaxLength(20), Required]
        public string PasswordHash { get; set; } = default!;
        public string PasswordSalt { get; set; } = default!;
        public string UserType { get; set; }
        [MaxLength(20)]
        public string? FirstName { get; set; }
        [MaxLength(20)]
        public string? LastName { get; set; }
    }
}
