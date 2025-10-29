using System;
using System.Collections.Generic;

namespace Application.DTOs
{
    public class EventResponse
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public required string Location { get; set; }
        public int Capacity { get; set; }
        public List<string> Participants { get; set; } = new();
        public bool IsOrganizer { get; set; }
        public bool IsParticipant { get; set; }
        public ICollection<TagDto>? Tags { get; set; }
    }

    public class EventSummaryResponse
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public string? ShortDescription { get; set; }
        public DateTime Date { get; set; }
        public required string Location { get; set; }
        public int Capacity { get; set; }
        public bool IsPublic { get; set; }
        public bool IsFull { get; set; }
        public int ParticipantCount { get; set; }
        public required string OrganizerName { get; set; }
        public bool IsParticipant { get; set; }
        public ICollection<TagDto>? Tags { get; set; }
    }

    public class CalendarEventResponse
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public DateTime Date { get; set; }
        public string Color { get; set; }
    }
}