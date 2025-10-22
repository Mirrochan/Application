using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    namespace Application.DTOs
    {
        public class RegisterRequest
        {
            public required string Email { get; set; }
            public required string Password { get; set; }
            public required string FirstName { get; set; }
            public required string LastName { get; set; }
        }

        public class LoginRequest
        {
            public required string Email { get; set; }
            public required string Password { get; set; }
        }

        public class UserResponse
        {
            public Guid Id { get; set; }
            public required string Email { get; set; }
            public required string FirstName { get; set; }
            public required string LastName { get; set; }
            public string FullName => $"{FirstName} {LastName}";
            public string Initials => $"{FirstName[0]}{LastName[0]}".ToUpper();
        }
    }
    namespace Application.DTOs
    {
        public class CreateEventRequest
        {
            public required string Title { get; set; }
            public string? Description { get; set; }
            public required DateTime StartAt { get; set; }
            public required DateTime EndAt { get; set; }
            public required string Location { get; set; }
            public int Capacity { get; set; }
            public bool IsPublic { get; set; } = true;
        }

        public class UpdateEventRequest
        {
            public string? Title { get; set; }
            public string? Description { get; set; }
            public DateTime? StartAt { get; set; }
            public DateTime? EndAt { get; set; }
            public string? Location { get; set; }
            public int? Capacity { get; set; }
            public bool? IsPublic { get; set; }
        }

        public class EventResponse
        {
            public Guid Id { get; set; }
            public required string Title { get; set; }
            public string? Description { get; set; }
            public DateTime StartAt { get; set; }
            public DateTime EndAt { get; set; }
            public required string Location { get; set; }
            public int Capacity { get; set; }
            public bool IsPublic { get; set; }
            public bool IsFull { get; set; }
            public int ParticipantCount { get; set; }
            public required UserResponse Organizer { get; set; }
            public List<UserResponse> Participants { get; set; } = new List<UserResponse>();
            public bool IsOrganizer { get; set; }
            public bool IsParticipant { get; set; }
        }

        public class EventSummaryResponse
        {
            public Guid Id { get; set; }
            public required string Title { get; set; }
            public string? ShortDescription { get; set; }
            public DateTime StartAt { get; set; }
            public DateTime EndAt { get; set; }
            public required string Location { get; set; }
            public int Capacity { get; set; }
            public bool IsPublic { get; set; }
            public bool IsFull { get; set; }
            public int ParticipantCount { get; set; }
            public required string OrganizerName { get; set; }
            public bool IsParticipant { get; set; }
        }

        public class CalendarEventResponse
        {
            public Guid Id { get; set; }
            public required string Title { get; set; }
            public DateTime StartAt { get; set; }
            public DateTime EndAt { get; set; }
            public required string Location { get; set; }
            public required string EventType { get; set; }
        }
    }
}
