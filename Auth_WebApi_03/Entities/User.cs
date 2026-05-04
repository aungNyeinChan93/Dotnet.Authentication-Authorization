using System.ComponentModel.DataAnnotations;

namespace Auth_WebApi_03.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}
