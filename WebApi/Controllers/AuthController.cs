using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebApi.Models.Dto;
using WebApi.Models.Entities;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("sliding")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;


        public AuthController(UserManager<User> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (dto.Password != dto.ConfirmPassword) return BadRequest("Confirm password is not match");
            var user = new User { UserName = dto.Email, Email = dto.Email };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok(new { message = "registered" });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return Unauthorized();
            var check = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!check) return Unauthorized();
            var tokens = await _tokenService.GenerateTokensAsync(user);
            return Ok(tokens);
        }


        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshRequest request)
        {
            var tokens = await _tokenService.RefreshAsync(request.RefreshToken);
            if (tokens == null) return Unauthorized();
            return Ok(tokens);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(LogoutRequest request)
        {
            var success = await _tokenService.RevokeRefreshTokenAsync(request.RefreshToken);
            if (!success) return BadRequest("Invalid refresh token");

            return Ok(new { message = "Logged out" });
        }
    }
}
