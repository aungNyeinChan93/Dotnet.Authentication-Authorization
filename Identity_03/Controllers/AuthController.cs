using Identity_03.Models;
using Identity_03.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity_03.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRequestModel userRequestModel)
        {
            var result = await _authService.RegisterAsync(userRequestModel);
            return Ok(result);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]UserLoginRequestModel userLoginRequestModel)
        {
            var token = await _authService.LoginAsync(userLoginRequestModel);
            if (token is null)
            {
                return BadRequest("login fail");
            }
            return Ok(token);
        }
    }
}
