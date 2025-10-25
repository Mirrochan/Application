using Application.Abstractions;
using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Event_Management_System.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            try
            {
                Response.Cookies.Delete("tasty-cookies");
                await _userService.RegisterAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                Response.Cookies.Delete("tasty-cookies");
                var token = await _userService.LoginAsync(request);

                Response.Cookies.Append("tasty-cookies", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // true у продакшні
                    SameSite = SameSiteMode.Strict,
                    MaxAge = TimeSpan.FromDays(7)
                });

                return Ok(new { message = "Logged in successfully" });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("tasty-cookies");
            return Ok(new { message = "Logout successful" });
        }

        [HttpGet("check")]
        public async Task<IActionResult> CheckAuth()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userService.GetUserByIdAsync(GetUserIdFromToken());
                if (user == null)
                {
                    return Unauthorized();
                }
                return Ok();
            }
              
            return Unauthorized();
        }
        
        private Guid GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new Exception("Unauthorized - userId claim not found");
            }

            return userId;
        }
    }
}
