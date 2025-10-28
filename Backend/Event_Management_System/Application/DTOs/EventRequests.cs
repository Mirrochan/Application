using System;

namespace Application.DTOs
{
    public class CreateEventRequest
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required DateTime Date { get; set; }
        public required string Location { get; set; }
        public int Capacity { get; set; }
        public bool IsPublic { get; set; } = true;
        public ICollection<Guid>? TagIds { get; set; }
    }

    public class UpdateEventRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
        public string? Location { get; set; }
        public int? Capacity { get; set; }
        public bool? IsPublic { get; set; }
        public ICollection<Guid>? TagIds { get; set; }
    }

 
}