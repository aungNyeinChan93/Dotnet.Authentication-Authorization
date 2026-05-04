using System.ComponentModel.DataAnnotations;

namespace Auth_WebApi_02.Models
{
    public class UserDto
    {
        //public int UserId { get; set; }

        public required string Name { get; set; }

        public required string Password { get; set; }
    }
}
