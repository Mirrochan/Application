

using Application.DTOs;

namespace Application.Abstractions
{
    public interface IEventService
    {
        Task<EventResponse> CreateEventAsync(CreateEventRequest request, Guid organizerId);
        Task DeleteEventAsync(Guid eventId, Guid userId);
        Task<EventResponse?> GetEventByIdAsync(Guid id, Guid? userId = null);
        Task<ICollection<MyEventsResponse>> GetMyEventsAsync(Guid userId);
        Task<ICollection<EventSummaryResponse>> GetPublicEventsAsync(Guid? userId = null);
        Task<ICollection<CalendarEventResponse>> GetUserEventsAsync(Guid userId);
        Task JoinEventAsync(Guid eventId, Guid userId);
        Task LeaveEventAsync(Guid eventId, Guid userId);
        Task<EventResponse> UpdateEventAsync(Guid eventId, UpdateEventRequest request, Guid userId);
    }
}