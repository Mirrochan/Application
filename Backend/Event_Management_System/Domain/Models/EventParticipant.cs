using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class EventParticipantModel
    {
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public EventModel Event { get; set; } = null!;
        public UserModel User { get; set; } = null!;
    }
}
