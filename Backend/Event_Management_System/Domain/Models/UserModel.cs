
namespace Domain.Models
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public ICollection<EventModel> OrganizedEvents { get; set; } = new List<EventModel>();
        public ICollection<EventModel> ParticipatingEvents { get; set; } = new List<EventModel>();
    }

}
