
namespace Domain.Models
{
    public class EventModel
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public required string Location { get; set; }
        public int Capacity { get; set; }
        public bool IsPublic { get; set; }
        public Guid OrganizerId { get; set; }
        public UserModel Organizer { get; set; } = null!;
        public ICollection<UserModel> Participants { get; set; } = new List<UserModel>();
    }
}
