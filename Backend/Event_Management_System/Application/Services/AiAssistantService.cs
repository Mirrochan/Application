using Application.Abstractions;
using Application.DTOs;
using Domain.Models;
using System.Text;

namespace Application.Services
{
    public class AiAssistantService : IAiAssistantService
    {
        private readonly IAiConversationRepository _conversationRepository;
        private readonly IAiProvider _aiProvider;
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITagRepository _tagRepository;

        public AiAssistantService(IAiConversationRepository conversationRepository, 
            IAiProvider aiProvider,
            IEventRepository eventRepository,
            IUserRepository userRepository,
            ITagRepository tagRepository
            )
        {
            _conversationRepository = conversationRepository;
            _aiProvider = aiProvider;
            _eventRepository = eventRepository;
            _userRepository = userRepository;
            _tagRepository = tagRepository;
        }

        public async Task<AskQuestionResponse> AskQuestionAsync(string request, Guid userId)
        {
            try
            {
                var aiResponse = AskQuestion(request, userId);
                var aiResponseResult = await _aiProvider.GetCompletionAsync(await aiResponse);
                var conversation = new AiConversation
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Question = request,
                    Answer = aiResponseResult,
                    AskedAt = DateTime.UtcNow
                };

                await _conversationRepository.AddAsync(conversation);


                return new AskQuestionResponse
                {
                    Answer = aiResponseResult,
                    AskedAt = conversation.AskedAt
                };
            }
            catch (Exception ex)
            {

                return new AskQuestionResponse
                {
                    Answer = "I'm having trouble processing your question right now. Please try again later.",
                    AskedAt = DateTime.UtcNow
                };
            }
        }

        private async Task<string> AskQuestion(string question, Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var userEvents = await _eventRepository.GetUserEventsAsync(userId);
            var publicEvents = await _eventRepository.GetAllPublicEventsAsync();
            var availableTags = await _tagRepository.GetAllTagsAsync();

            var User = new UserContext
            {
                Id = user.Id,
                Name = $"{user.FirstName} {user.LastName}",
                Email = user.Email
            };

            var UserEvents = userEvents.Select(e => new EventContext
            {
                Id = e.Id,
                Title = e.Title,
                StartAt = e.Date,
                Location = e.Location,
                IsPublic = e.IsPublic,
                OrganizerId = e.OrganizerId,
                OrganizerName = $"{e.Organizer.FirstName} {e.Organizer.LastName}",
                Tags = e.Tags.Select(t => t.Name).ToList(),
                ParticipantNames = e.Participants.Select(p => $"{p.FirstName} {p.LastName}").ToList()
            }).ToList();

            var PublicEvents = publicEvents.Select(e => new EventContext
            {
                Id = e.Id,
                Title = e.Title,
                StartAt = e.Date,
                Location = e.Location,
                IsPublic = e.IsPublic,
                OrganizerId = e.OrganizerId,
                OrganizerName = $"{e.Organizer.FirstName} {e.Organizer.LastName}",
                Tags = e.Tags.Select(t => t.Name).ToList(),
                ParticipantNames = e.Participants.Select(p => $"{p.FirstName} {p.LastName}").ToList()
            }).ToList();

            var AvailableTags = availableTags.Select(t => new TagContext
            {
                Name = t.Name,
                Color = t.Color
            }).ToList();

            string FormatEvents(List<EventContext> events)
            {
                if (!events.Any())
                    return "None.\n";

                var sb = new StringBuilder();
                foreach (var e in events)
                {
                    sb.AppendLine($"- {e.Title} ({e.StartAt:yyyy-MM-dd HH:mm}) at {e.Location}");
                    sb.AppendLine($"  Organizer: {e.OrganizerName}");
                    sb.AppendLine($"  Tags: {string.Join(", ", e.Tags)}");
                    sb.AppendLine($"  Participants: {string.Join(", ", e.ParticipantNames)}");
                    sb.AppendLine();
                }
                return sb.ToString();
            }

            return $"""
                You are a **helpful AI assistant** for an **Event Management System**.

                Your task is to answer natural-language questions about events using the provided data.
                You have **read-only access** — you may NOT create, edit, or delete any information.

                CONTEXT

                User
                Name: {User.Name}
                Email: {User.Email}

                User's Events ({UserEvents.Count})
                {FormatEvents(UserEvents)}

                Public Events ({PublicEvents.Count})
                {FormatEvents(PublicEvents)}

                Available Tags
                {string.Join(", ", AvailableTags.Select(t => t.Name))}

                USER QUESTION -
                {question}

                 INSTRUCTIONS
                - Answer naturally and concisely in full sentences.
                - Use **only** the information in the provided context.
                - If the question is about:
                    - “events I’m attending” → use the user's event list.
                    - “events I organize” → filter by OrganizerId == User.Id.
                    - “public tech/design/marketing events” → filter Public Events by matching tag name.
                    - “who’s attending” → list participants for the specified event.
                    - “when/where is…” → provide event name, date, and location.
                - If the answer cannot be found, reply:
                  “Sorry, I couldn’t find that information in the current event data.”
                - Do not make up information or infer missing data.
                - Return a **clear text answer** only (no JSON, no code).
                ANSWER:
                """;
                }


        public async Task<List<ConversationHistoryResponse>> GetConversationHistoryAsync(Guid userId)
        {
            var conversations = await _conversationRepository.GetUserConversationsAsync(userId);

            return conversations.Select(c => new ConversationHistoryResponse
            {
                Id = c.Id,
                Question = c.Question,
                Answer = c.Answer,
                AskedAt = c.AskedAt
            }).ToList();
        }
    }
}