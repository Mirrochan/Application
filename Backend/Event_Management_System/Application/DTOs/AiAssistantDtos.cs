using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class AskQuestionRequest
    {
        public required string Question { get; set; }
    }

    public class AskQuestionResponse
    {
        public required string Answer { get; set; }
        public DateTime AskedAt { get; set; } = DateTime.UtcNow;
    }

    public class ConversationHistoryResponse
    {
        public Guid Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public DateTime AskedAt { get; set; }
    }

    public class UserContext
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class EventContext
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string Location { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public Guid OrganizerId { get; set; }
        public string OrganizerName { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
        public List<string> ParticipantNames { get; set; } = new();
    }

    public class TagContext
    {
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }
    public class AiAssistantQuestionRequest
    {
        public required string Question { get; set; }
    }
}
