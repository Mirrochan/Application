using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class TagModel
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string Color { get; set; } = string.Empty;
        public ICollection<EventModel> Events { get; set; } = new List<EventModel>();
    }
}
