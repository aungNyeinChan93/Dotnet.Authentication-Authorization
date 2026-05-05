using Identity_03.Data;
using Identity_03.Entity;
using Identity_03.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;

namespace Identity_03.Services
{
    public class AuthService
    {

        private readonly UserManager<AppUser> _userManager;

        private readonly IConfiguration configuration;

        public AuthService(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            this.configuration = configuration;
        }

        public async Task<AppUser?> RegisterAsync(UserRequestModel userRequestModel)
        {
            var user = new AppUser()
            {
                UserName = userRequestModel.Name,
                Email = userRequestModel.Email,
            };

            var result = await _userManager.CreateAsync(user,userRequestModel.Password);

            if (result.Succeeded)
            {
                return user;
            }
            return default!;
        }

        public async Task<TokenResponseModel?> LoginAsync(UserLoginRequestModel requestModel)
        {
            var user =await _userManager.FindByEmailAsync(requestModel.Email);
            if (user is null)
            {
                return default!;
            }
            var accessToken = await this.GenerateAccessToken(user);
            return accessToken is not null ? new TokenResponseModel { AccessToken = accessToken} : default!;
        }



        private async Task<string?> GenerateAccessToken(AppUser appUser)
        {
            try
            {
                if (appUser is null || appUser.Email == null)
                {
                    return default!;
                }

                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:JWT_SECRET")!));

                var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new ClaimsIdentity(new List<Claim>
                {
                    new Claim("userId",appUser.Id.ToString()),
                    new Claim("email",appUser.Email.ToString())
                });

                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = credential,
                };

                var jwtTokenHandler = new JwtSecurityTokenHandler();
                var securityToken = jwtTokenHandler.CreateToken(tokenDescriptor);
                var token = jwtTokenHandler.WriteToken(securityToken);

                return token;
            }
            catch (Exception err)
            {
                return err.Message;
            }
        }
    }
}
