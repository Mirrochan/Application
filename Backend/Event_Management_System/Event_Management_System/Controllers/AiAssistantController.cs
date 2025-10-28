using Application.Abstractions;
using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Event_Management_System.Controllers
{
    [Route("ai-assistant")]
    public class AiAssistantController : Controller
    { 
        private readonly IAiAssistantService _aiAssistantService;
        private readonly IUserService _userService;
        public AiAssistantController(IAiAssistantService aiAssistantService, IUserService userService)
        {
            _aiAssistantService = aiAssistantService;
            _userService = userService;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> AskQuestion([FromBody] AiAssistantQuestionRequest request)
        {
            try
            {
                var userId = await GetUserIdFromToken();
                Console.WriteLine("UserId from token: " + userId);
                if (userId == Guid.Empty)
                    return Unauthorized();
                var response = await _aiAssistantService.AskQuestionAsync(request.Question,userId);
                return Ok(response);

            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            
        }
        [HttpGet("history")]
        public async Task<IActionResult> GetConversationHistory()
        {
            try
            {
                var userId = await GetUserIdFromToken();
                
                if (userId == Guid.Empty) 
                {  
                    return Unauthorized(); 
                }
                var history = await _aiAssistantService.GetConversationHistoryAsync(userId);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        private async Task<Guid> GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            Console.WriteLine("UserId from token: " + userIdClaim);
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Guid.Empty;
            }
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return Guid.Empty;
            }
            return userId;
        }

    }
}
