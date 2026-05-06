using Identity_03.Data;
using Identity_03.Entity;
using Identity_03.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
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


        //Register
        public async Task<AppUser?> RegisterAsync(UserRequestModel userRequestModel)
        {
            var user = new AppUser()
            {
                UserName = userRequestModel.Name,
                Email = userRequestModel.Email,
                LibraryId = userRequestModel.LibraryId,
            };

            var result = await _userManager.CreateAsync(user,userRequestModel.Password);

            if (!result.Succeeded) return default!;
            
            var res = await _userManager.AddToRoleAsync(user,userRequestModel.UserRole.ToString());
            if (res.Succeeded)
            {
                return user;
            }

            return default!;
        }


        //Login
        public async Task<TokenResponseModel?> LoginAsync(UserLoginRequestModel requestModel)
        {
            var user =await _userManager.FindByEmailAsync(requestModel.Email);

            if (user is null) return default!;

            var isValidPassword = await _userManager.CheckPasswordAsync(user, requestModel.Password);
            
            if(!isValidPassword) return default!;

            var accessToken = await this.GenerateAccessToken(user);

            return accessToken is not null ? new TokenResponseModel { AccessToken = accessToken} : default!;
        }

        private async Task<string?> GenerateAccessToken(AppUser appUser, IOptions<AppSettings>? appSettings = null)
        {
            try
            {
                if (appUser is null || appUser.Email == null)
                {
                    return default!;
                }

                var roles =await _userManager.GetRolesAsync(appUser);

                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:JWT_SECRET")!));
                //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Value.JWT_SECRET!));

                var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier ,appUser.Id),
                    new Claim(ClaimTypes.Role , roles.First()),
                    new Claim("userId",appUser.Id.ToString()),
                    new Claim(ClaimTypes.Email,appUser.Email),
                    //new Claim("email",appUser.Email),
                });

                if (appUser.LibraryId is not null)
                {
                    claims.AddClaim(new Claim("libraryId", appUser.LibraryId.ToString()!));
                }

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
