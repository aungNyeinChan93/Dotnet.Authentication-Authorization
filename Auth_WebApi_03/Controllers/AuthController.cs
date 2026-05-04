using Auth_WebApi_03.Models;
using Auth_WebApi_03.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Auth_WebApi_03.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            var user = await _authService.RegisterAsync(userDto);
            if (user is null)
            {
                return BadRequest();
            }
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDto userDto)
        {
            var token = await _authService.LoginAsync(userDto);
            if (token is null)
            {
                return BadRequest();
            }
            return Ok(new {token});
        }

        [Authorize]
        [HttpGet("isAuth")]
        public async Task<IActionResult> IsAuthenticate()
        {
            return Ok("You are authrnticate user");
        }
    }
}
