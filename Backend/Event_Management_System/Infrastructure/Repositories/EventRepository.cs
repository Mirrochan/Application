using Application.Abstractions;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly EventManagmentSystemDbContext _context;

        public EventRepository(EventManagmentSystemDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<EventModel>> GetAllPublicEventsAsync()
        {
            return await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Participants)
                .Where(e => e.IsPublic == true && e.EndAt >= DateTime.UtcNow)
                .OrderBy(e => e.StartAt)
                .ToListAsync();
        }

        public async Task<EventModel?> GetByIdAsync(Guid id)
        {
            return await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Participants)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task AddAsync(EventModel eventModel)
        {
            await _context.Events.AddAsync(eventModel);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(EventModel eventModel)
        {
            _context.Events.Update(eventModel);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var eventEntity = await _context.Events.FindAsync(id);
            if (eventEntity != null)
            {
                _context.Events.Remove(eventEntity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ICollection<EventModel>> GetUserEventsAsync(Guid userId)
        {
            return await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Participants)
                .Where(e => e.Participants.Any(p => p.Id == userId) || e.OrganizerId == userId)
                .OrderBy(e => e.StartAt)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Events.AnyAsync(e => e.Id == id);
        }
    }
}