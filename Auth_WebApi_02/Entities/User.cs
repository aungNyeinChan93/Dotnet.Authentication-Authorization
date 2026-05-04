using System.ComponentModel.DataAnnotations;

namespace Auth_WebApi_02.Entities
{
    public class User
    {
        //[Key]
        public int UserId { get; set; }

        //[Required]
        public  string? Name { get; set; }

        //[Required]
        //[MinLength(6)]
        //[MaxLength(255)]
        public  string?  Password { get; set; }
    }
}
