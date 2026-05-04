using Auth_WebApi_03.Data;
using Auth_WebApi_03.Entities;
using Auth_WebApi_03.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth_WebApi_03.Services
{
    public class AuthService : IAuthService
    {

        private readonly AppdbContext _context;

        private readonly IConfiguration configuration;

        public AuthService(AppdbContext context, IConfiguration configuration)
        {
            _context = context;
            this.configuration = configuration;
        }


        public async Task<User?> RegisterAsync(UserDto userDto)
        {
            var isExist = await _context.Users.AsNoTracking().AnyAsync(u => u.Name == userDto.Name);
            if (isExist)
            {
                return default!;
            }
            var user = new User() { Name = "",Password ="" };
            var hashPassword = new PasswordHasher<User>().HashPassword(user, userDto.Password);

            user.Name = userDto.Name;
            user.Password = hashPassword;
            _context.Users.Add(user);
            var result = await _context.SaveChangesAsync();
            return result >=1 ? user : default!;

        }

        public async Task<string?> LoginAsync(UserDto userDto)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u=>u.Name == userDto.Name);

            if (user is null)
            {
                return default!;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier , user.UserId.ToString()),
                new Claim(ClaimTypes.Name , user.Name),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

            var cred = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(
                    issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                    audience: configuration.GetValue<string>("AppSettings:Audience"),
                    claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials:cred
                );

            var tokenStr = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            return tokenStr;
        }

    }
}
