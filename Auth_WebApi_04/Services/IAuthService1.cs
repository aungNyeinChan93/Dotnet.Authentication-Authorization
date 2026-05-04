using Auth_WebApi_04.Entity;
using Auth_WebApi_04.Models;

namespace Auth_WebApi_04.Services
{
    public interface IAuthService1
    {
        Task<string?> LoginAsync(UserDto userDto);
        Task<User?> RegisterAsync(UserDto userDto);
    }
}