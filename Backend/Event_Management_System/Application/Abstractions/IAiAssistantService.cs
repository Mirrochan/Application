using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions
{
    public interface IAiAssistantService
    {
        Task<AskQuestionResponse> AskQuestionAsync(string request, Guid userId);
        Task<List<ConversationHistoryResponse>> GetConversationHistoryAsync(Guid userId);
    }
}
