using Application.Abstractions;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class AiConversationRepository: IAiConversationRepository
    {
        private readonly EventManagmentSystemDbContext _context;

        public AiConversationRepository(EventManagmentSystemDbContext context)
        {
            _context = context;
        }

        public async Task<AiConversation> AddAsync(AiConversation conversation)
        {
            await _context.AiConversations.AddAsync(conversation);
            await _context.SaveChangesAsync();
            return conversation;
        }

        public async Task<List<AiConversation>> GetUserConversationsAsync(Guid userId)
        {
            return await _context.AiConversations
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.AskedAt)
                .ToListAsync();
        }
    }
}
