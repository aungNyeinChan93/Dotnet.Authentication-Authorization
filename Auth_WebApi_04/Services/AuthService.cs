using Auth_WebApi_04.Data;
using Auth_WebApi_04.Entity;
using Auth_WebApi_04.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Auth_WebApi_04.Services
{
    public class AuthService :IAuthService
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

        public async Task<TokenResponeModel?> LoginAsync(UserDto userDto)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == userDto.Email);

            if (user is null)
            {
                return default!;
            }

            var refreshToke = await GenerateRereshToken(user);
            var accessToken = await GenerateAccessToken(user);

            var responseModel = new TokenResponeModel()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToke,
            };

            return responseModel;
        }

        public async Task<TokenResponeModel?> RefreshTokenAsync(UserDto userDto)
        {

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);
            if (user is null || user.RefreshToken == null || user.UserId < 0)
            {
                return default!;
            }

            var model = new RefershTokenRequestModel()
            {
                RefreshToken = user.RefreshToken,
                UserId = user.UserId,
            };

            var validateUser = await this.ValidateRefreshToken(model);

            if (validateUser is null)
            {
                return default;
            }

            var accessToken = await this.GenerateAccessToken(validateUser);
            var refreshToken = await this.GenerateRereshToken(validateUser);

            return new TokenResponeModel()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };

        }



        private async Task<string?> GenerateRereshToken(User user)
        {
            var ramdomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(ramdomNumber);
            var refreshToken = Convert.ToBase64String(ramdomNumber);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpire = DateTime.Now.AddDays(7);
            _context.Entry(user).State = EntityState.Modified;
            var result = await _context.SaveChangesAsync();
            return result >= 1 ? refreshToken : null;

        }

        private async Task<string?> GenerateAccessToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email ,user!.Email),
                new Claim(ClaimTypes.Name ,user.Name),
                new Claim(ClaimTypes.NameIdentifier ,user.UserId.ToString()),
                new Claim(ClaimTypes.Role,user.Role.ToString())
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

            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
            return accessToken;
        }

        private async Task<User?> ValidateRefreshToken(RefershTokenRequestModel request)
        {
            if (request.RefreshToken == null || request.UserId < 0)
            {
                return default!;
            }

            var user = await _context.Users.FindAsync(request.UserId);

            if (user is null)
            {
                return default!;
            }

            if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpire < DateTime.Now)
            {
                return default!;
            }

            return user;
        }
    }
}
