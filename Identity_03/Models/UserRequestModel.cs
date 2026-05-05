using Microsoft.AspNetCore.Identity;

namespace Identity_03.Models
{
    public class UserRequestModel
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
