using Application.Abstractions;
using Application.DTOs;
using Application.DTOs.Application.DTOs;
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

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPublicEvents()
        {
            var userId = GetUserIdFromToken();
            var events = await _eventService.GetPublicEventsAsync(userId);
            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(string id)
        {
            if (!Guid.TryParse(id, out Guid eventId))
            {
                return BadRequest(new { error = "Invalid event ID" });
            }

            var userId = GetUserIdFromToken();
            var eventItem = await _eventService.GetEventByIdAsync(eventId, userId);

            return eventItem != null ? Ok(eventItem) : NotFound();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateEvent(CreateEventRequest request)
        {
            try
            {
                Guid userId = GetUserIdFromToken();
                var result = await _eventService.CreateEventAsync(request, userId);
                return CreatedAtAction(nameof(GetEventById), new { id = result.Id }, result);
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
                var userId = GetUserIdFromToken();
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
                var userId = GetUserIdFromToken();
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
                var userId = GetUserIdFromToken();
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
                Guid userId = GetUserIdFromToken();
                await _eventService.LeaveEventAsync(eventId, userId);
                return Ok(new { message = "Successfully left the event" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
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