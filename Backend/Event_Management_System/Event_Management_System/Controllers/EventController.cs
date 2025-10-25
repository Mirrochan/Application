using Application.Abstractions;
using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace Event_Management_System.Controllers
{
    [ApiController]
    [Route("events")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IUserService _userService;

        public EventsController(IEventService eventService, IUserService userService)
        {
            _eventService = eventService;
            _userService = userService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPublicEvents()
        {
            var userId = await GetUserIdFromToken();
            if (userId == Guid.Empty)
                return Unauthorized(); 
            var events = await _eventService.GetPublicEventsAsync(userId);
            return Ok(events);
        }
        
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetEventById(string id)
        {
            if (!Guid.TryParse(id, out Guid eventId))
            {
                return BadRequest(new { error = "Invalid event ID" });
            }

            var userId = await GetUserIdFromToken();
            if (userId == Guid.Empty)
                return Unauthorized(); 
            var eventItem = await _eventService.GetEventByIdAsync(eventId, userId);

            return eventItem != null ? Ok(eventItem) : NotFound();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateEvent(CreateEventRequest request)
        {
            try
            {
                var userId = await GetUserIdFromToken();
                if (userId == Guid.Empty)
                    return Unauthorized(); 
                var result = await _eventService.CreateEventAsync(request, userId);
                return Ok(  result );
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateEvent(string id, UpdateEventRequest request)
        {
            if (!Guid.TryParse(id, out Guid eventId))
            {
                return BadRequest(new { error = "Invalid event ID" });
            }

            try
            {
                var userId = await GetUserIdFromToken();
                if (userId == Guid.Empty)
                    return Unauthorized(); 
                var result = await _eventService.UpdateEventAsync(eventId, request, (Guid)userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteEvent(string id)
        {
            if (!Guid.TryParse(id, out Guid eventId))
            {
                return BadRequest(new { error = "Invalid event ID" });
            }

            try
            {
                var userId = await GetUserIdFromToken();
                if (userId == Guid.Empty)
                    return Unauthorized(); 
                await _eventService.DeleteEventAsync(eventId, (Guid)userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{id}/join")]
        [Authorize]
        public async Task<IActionResult> JoinEvent(string id)
        {
            if (!Guid.TryParse(id, out Guid eventId))
            {
                return BadRequest(new { error = "Invalid event ID" });
            }

            try
            {
                var userId = await GetUserIdFromToken();
                if (userId == Guid.Empty)
                    return Unauthorized();
                await _eventService.JoinEventAsync(eventId, (Guid)userId);
                return Ok(new { message = "Successfully joined the event" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{id}/leave")]
        [Authorize]
        public async Task<IActionResult> LeaveEvent(string id)
        {
            if (!Guid.TryParse(id, out Guid eventId))
            {
                return BadRequest(new { error = "Invalid event ID" });
            }

            try
            {
                var userId = await GetUserIdFromToken();
                if (userId == Guid.Empty)
                    return Unauthorized(); 
                await _eventService.LeaveEventAsync(eventId, userId);
                return Ok(new { message = "Successfully left the event" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        private async Task<Guid> GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Guid.Empty;
            }
            var user = await _userService.GetUserByIdAsync(userId);
            if(user == null)
            {
                return Guid.Empty;
            }
            return userId;
        }


    }
}