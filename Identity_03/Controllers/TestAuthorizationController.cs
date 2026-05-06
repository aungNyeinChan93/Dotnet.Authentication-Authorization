using Identity_03.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Identity_03.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestAuthorizationController : ControllerBase
    {

        [Authorize(Roles ="3")]
        [HttpGet("Admin-Only")]
        public async Task<IActionResult> AdminOnly()
        {
            return Ok("Admin only!");
        }

        [Authorize(Roles = "2")]
        [HttpGet("User-Only")]
        public async Task<IActionResult> UserOnly()
        {
            return Ok("User only!");
        }

        [Authorize]
        [HttpGet("check-claims")]
        public async Task<IActionResult> TestClaims()
        {
            //var claims = User.Claims.ToList();
            //var userId = User.Claims.FirstOrDefault(x=> x.Type == "userId")!.Value!;
            //var email = User.Claims.FirstOrDefault(x=>x.Type == "email")!.Value!;
            var userId = User.FindFirst("userId")!.Value;
            var email = User.FindFirstValue(ClaimTypes.Email);
            var role = User.FindFirstValue(ClaimTypes.Role);
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok(new { role ,email ,id });
        }


        [Authorize(Roles ="3,4")]
        [HttpGet("only-admin&super")]
        public async Task<IActionResult> OnlyAdminAndSuper()
        {
            return Ok("Admin and Super!");
        }

        [Authorize(Policy = "HasLibraryId")]
        [HttpGet("IsHasLibraryId")]
        public async Task<IActionResult> IshasLibraryId()
        {
            var libraryId = User.FindFirstValue("libraryId");
            return Ok(new { libraryId });
        }

    }
}
