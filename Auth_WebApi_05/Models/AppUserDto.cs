using System.ComponentModel.DataAnnotations;

namespace Auth_WebApi_05.Models
{
    public class AppUserDto
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Email { get; set; }

        public string? Password { get; set; } = null;
    }
}
