using Identity_03.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Identity_03.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountsController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProfile(UserManager<AppUser> userManager)
        {
            var u = User;
            //var userId = _httpContextAccessor.HttpContext!.User.Claims.First(x => x.Type == "userId")!.Value;
            //var userId = User.Claims.First(x => x.Type == "userId")!.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await userManager.FindByIdAsync(userId);
            return Ok(new { user });
        }
    }
}
