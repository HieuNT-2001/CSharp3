using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebApi.Models.Entities;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("sliding")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;


        public UserController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }


        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var email = User.Identity?.Name ?? User.FindFirst("email")?.Value;
            if (email == null) return Unauthorized();
            var u = await _userManager.FindByEmailAsync(email);
            if (u == null) return NotFound();
            return Ok(new { u.Id, u.UserName, u.Email });
        }
    }
}
