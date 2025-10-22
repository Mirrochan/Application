using Domain.Models;

namespace Application.Abstractions
{
    public interface IEventRepository
    {
        Task AddAsync(EventModel eventModel);
        Task DeleteAsync(Guid id);
        Task<ICollection<EventModel>> GetAllPublicEventsAsync();
        Task<EventModel?> GetByIdAsync(Guid id);
        Task UpdateAsync(EventModel eventModel);
        Task<ICollection<EventModel>> GetUserEventsAsync(Guid userId);
        Task<bool> ExistsAsync(Guid id);
    }
}