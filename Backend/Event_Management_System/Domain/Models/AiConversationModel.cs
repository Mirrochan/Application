namespace Domain.Models
{
    public class AiConversation
    {
        public Guid Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public DateTime AskedAt { get; set; } = DateTime.UtcNow;
        public Guid UserId { get; set; }
        public UserModel User { get; set; } = null!;
    }
}