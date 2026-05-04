using Auth_WebApi_03.Entities;
using Auth_WebApi_03.Models;

namespace Auth_WebApi_03.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDto userDto);

        Task<string?> LoginAsync(UserDto userDto);
    }
}
