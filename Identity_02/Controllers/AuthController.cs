using Identity_02.Entities;
using Identity_02.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Identity_02.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IConfiguration configuration;

        public AuthController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup(UserManager<AppUser> userManager, [FromBody]UserRegisterationModel userRegisterationModel)
        {

            var user = new AppUser
            {
                UserName = userRegisterationModel.FullName,
                Email = userRegisterationModel.Email,
                FullName = userRegisterationModel.FullName,
            };

            var result = await userManager.CreateAsync(user,userRegisterationModel.Password);


            if (result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody]LoginRequestModel loginRequestModel,
            UserManager<AppUser> userManager)
        {
            var user = await userManager.FindByEmailAsync(loginRequestModel.Email);

            if (user is null || user.Email == null)
            {
                return BadRequest("email and password are incorrect!");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:JWT_SECRET")!));

            var claims = new ClaimsIdentity(new List<Claim>
            {
                new Claim("userId" ,user.Id.ToString()),
                new Claim("email",user.Email.ToString())
            });

            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = claims,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credential, 
            };

            var securityToken = new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);
            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            if (token is null)
            {
                return BadRequest("token generate fail");
            }

            return Ok(new {token});
        }
    }
}
