using Auth_WebApi_02.Entities;
using Auth_WebApi_02.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth_WebApi_02.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IConfiguration configuration;

        public AuthController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        private static User testUser = new();

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            var hashPasswod = new PasswordHasher<User>().HashPassword(testUser,userDto.Password);
            testUser.Name = userDto.Name;
            testUser.Password = hashPasswod;
            return Ok(testUser);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDto userDto)
        {
            if (userDto.Name != testUser.Name)
            {
                return BadRequest();
            }

            if (new PasswordHasher<User>().VerifyHashedPassword(testUser,testUser.Password,userDto.Password) 
                == PasswordVerificationResult.Failed)
            {
                return BadRequest();
            }

            string token = GenerateToken(userDto);
            return Ok(token);
        }

        private string GenerateToken(UserDto userDto)
        {
            /*
            1 - make claim 
            2 - make key
            3 - make cred
            4 - make toekndescriptor
            */

            var claims = new List<Claim>
            {
                new Claim (ClaimTypes.Name ,userDto.Name)
            };

            var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!)
                );

            var cred = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(
                issuer:configuration.GetValue<string>("AppSettings:Issuer"),
                audience:configuration.GetValue<string>("AppSettings:Audience")!,
                claims:claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials:cred
                );
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
