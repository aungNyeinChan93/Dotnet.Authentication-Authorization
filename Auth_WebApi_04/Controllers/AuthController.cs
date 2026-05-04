using Auth_WebApi_04.Entity;
using Auth_WebApi_04.Models;
using Auth_WebApi_04.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Auth_WebApi_04.Controllers
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


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserDto userDto)
        {
            var user = await _authService.RegisterAsync(userDto);
            if (user is null)
            {
                return BadRequest("Account is already exist");
            }
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]UserDto userDto)
        {
            var result = await _authService.LoginAsync(userDto);
            if (result is null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpGet("isAuth")]
        [Authorize]
        public async Task<IActionResult> IsAuthenticate()
        {
            return Ok("You are Authenticate!");
        }

        [Authorize(Roles ="Admin")]
        [HttpGet("isAdmin")]
        public async Task<IActionResult> IsAdmin()
        {
            return Ok("YOu are admin!");
        }


        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody]UserDto user)
        {
            var result = await _authService.RefreshTokenAsync(user);
            if (result is null || result.RefreshToken == null || result.AccessToken == null)
            {
                return Unauthorized();
            }

            return Ok(result);
        }
    }
}
