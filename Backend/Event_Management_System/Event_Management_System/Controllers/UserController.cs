using Application.Abstractions;
using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Event_Management_System.Controllers
{
    [Route("users")]
    public class UserController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IUserService _userService;

        public UserController(IEventService eventService, IUserService userService)
        {
            _eventService = eventService;
            _userService = userService;
        }
        [HttpGet("me/events")]
        [Authorize]
        public async Task<IActionResult> GetMyEvents()
        {
            var userId = GetUserIdFromToken();
            var events = await _eventService.GetMyEventsAsync(userId);
            return Ok(events);
        }
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetUserInfo()
        {
            var userId = GetUserIdFromToken();
            UserResponse user = await _userService.GetUserByIdAsync(GetUserIdFromToken());
            if (user == null)
            {
                    return Unauthorized();
            }

            return Ok(new { name = user.FirstName });

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
