namespace Auth_WebApi_04.Models
{
    public class RefershTokenRequestModel
    {
        public int UserId { get; set; }
        public required string RefreshToken { get; set; }
    }
}
