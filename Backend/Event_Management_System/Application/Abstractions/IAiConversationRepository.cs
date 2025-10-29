using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions
{
    public interface IAiConversationRepository
    {
        Task<AiConversation> AddAsync(AiConversation conversation);
        Task<List<AiConversation>> GetUserConversationsAsync(Guid userId);
    }
}
