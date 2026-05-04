using Auth_WebApi_04.Entity;
using Auth_WebApi_04.Models;

namespace Auth_WebApi_04.Services
{
    public interface IAuthService
    {
        Task<TokenResponeModel?> LoginAsync(UserDto userDto);
        Task<TokenResponeModel?> RefreshTokenAsync(UserDto userDto);
        Task<User?> RegisterAsync(UserDto userDto);
    }
}