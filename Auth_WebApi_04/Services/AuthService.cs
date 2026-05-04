using Auth_WebApi_04.Data;
using Auth_WebApi_04.Entity;
using Auth_WebApi_04.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth_WebApi_04.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            this.configuration = configuration;
        }

        public async Task<User?> RegisterAsync(UserDto userDto)
        {
            var isUserExist = await _context.Users.AnyAsync(u => u.Email == userDto.Email);
            if (isUserExist)
            {
                return default!;
            }
            var user = new User() { Password = userDto.Password, Email = userDto.Email, Name = userDto.Name };
            var hashPassword = new PasswordHasher<User>().HashPassword(user, user.Password);
            user.Password = hashPassword;

            _context.Add(user);
            var result = await _context.SaveChangesAsync();
            return result >= 1 ? user : default!;
        }

        public async Task<string?> LoginAsync(UserDto userDto)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == userDto.Email);

            if (user is null)
            {
                return default!;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email ,user!.Email),
                new Claim(ClaimTypes.Name ,user.Name),
                new Claim(ClaimTypes.NameIdentifier ,user.UserId.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credential,
                    issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                    audience: configuration.GetValue<string>("AppSettings:Audience")
                );

            var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
            return token;
        }
    }
}
