using Application.Abstractions;
using Application.DTOs;
using Application.Validators;
using Domain.Models;
using FluentValidation;

namespace Application.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IValidator<CreateEventRequest> _createEventValidator;
        private readonly IValidator<UpdateEventRequest> _updateEventValidator;
        private readonly ITagRepository _tagRepository;

        public EventService(
            IEventRepository repository,
            IUserRepository userRepository,
            IValidator<CreateEventRequest> createEventValidator,
            IValidator<UpdateEventRequest> updateEventValidator,
            ITagRepository tagRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
            _createEventValidator = createEventValidator;
            _updateEventValidator = updateEventValidator;
            _tagRepository = tagRepository;
        }

        public async Task<ICollection<EventSummaryResponse>> GetPublicEventsAsync(Guid? userId)
        {
            var events = await _repository.GetAllPublicEventsAsync();
            var eventResponses = new List<EventSummaryResponse>();

            foreach (var eventModel in events)
            {
                var participantCount = eventModel.Participants.Count;
                var isFull = eventModel.Capacity > 0 && participantCount >= eventModel.Capacity;
                var isParticipant = userId.HasValue && eventModel.Participants.Any(p => p.Id == userId.Value);

                eventResponses.Add(new EventSummaryResponse
                {
                    Id = eventModel.Id,
                    Title = eventModel.Title,
                    ShortDescription = GetShortDescription(eventModel.Description),
                    Date = eventModel.Date,
                    Location = eventModel.Location,
                    Capacity = eventModel.Capacity,
                    IsPublic = eventModel.IsPublic,
                    IsFull = isFull,
                    ParticipantCount = participantCount,
                    OrganizerName = $"{eventModel.Organizer.FirstName} {eventModel.Organizer.LastName}",
                    IsParticipant = isParticipant,
                    Tags = eventModel.Tags.Select(t => new TagDto { Id = t.Id, Name = t.Name, Color = t.Color}).ToList()
                });
            }

            return eventResponses;
        }

        public async Task<EventResponse?> GetEventByIdAsync(Guid id, Guid? userId = null)
        {
            var eventModel = await _repository.GetByIdAsync(id);
            if (eventModel == null) return null;

            var participantCount = eventModel.Participants.Count;
            var isFull = eventModel.Capacity > 0 && participantCount >= eventModel.Capacity;
            var isOrganizer = userId.HasValue && eventModel.OrganizerId == userId.Value;
            var isParticipant = userId.HasValue && eventModel.Participants.Any(p => p.Id == userId.Value);

            List<string> participantResponses = eventModel.Participants.Select(p => p.FirstName).ToList();
            

            return new EventResponse
            {
                Id = eventModel.Id,
                Title = eventModel.Title,
                Description = eventModel.Description,
                Date = eventModel.Date,
                Location = eventModel.Location,
                Capacity = eventModel.Capacity,
                Participants = participantResponses,
                IsOrganizer = isOrganizer,
                IsParticipant = isParticipant,
                Tags = eventModel.Tags.Select(t => new TagDto { Id = t.Id, Name = t.Name, Color = t.Color }).ToList()

            };
        }

        public async Task<EventResponse> CreateEventAsync(CreateEventRequest request, Guid organizerId)
        {
            var validationResult = await _createEventValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var organizer = await _userRepository.GetByIdAsync(organizerId);
            if (organizer == null)
            {
                throw new Exception("Organizer not found");
            }

            var tags = new List<TagModel>();
            if (request.TagIds != null)
            {
                foreach (var tagId in request.TagIds)
                {
                    var tag = await _tagRepository.GetTagByIdAsync(tagId);
                    if (tag != null)
                    {
                        tags.Add(tag);
                    }
                }
            }

            var eventModel = new EventModel
            {
                Id = Guid.NewGuid(),
                Title = request.Title.Trim(),
                Description = request.Description?.Trim(),
                Date = request.Date,
                Location = request.Location.Trim(),
                Capacity = request.Capacity,
                IsPublic = request.IsPublic,
                OrganizerId = organizerId,
                Tags = tags
            };

            await _repository.AddAsync(eventModel);
            return await GetEventByIdAsync(eventModel.Id, organizerId);
        }

        public async Task<EventResponse> UpdateEventAsync(Guid eventId, UpdateEventRequest request, Guid userId)
        {
            var validationResult = await _updateEventValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var eventModel = await _repository.GetByIdAsync(eventId);
            if (eventModel == null)
            {
                throw new Exception("Event not found");
            }

            if (eventModel.OrganizerId != userId)
            {
                throw new Exception("You can only edit events you organized");
            }

            if (!string.IsNullOrWhiteSpace(request.Title))
                eventModel.Title = request.Title.Trim();

            if (request.Description != null)
                eventModel.Description = request.Description.Trim();

            if (request.Date.HasValue)
                eventModel.Date = request.Date.Value;

            if (!string.IsNullOrWhiteSpace(request.Location))
                eventModel.Location = request.Location.Trim();

            if (request.Capacity.HasValue)
                eventModel.Capacity = request.Capacity.Value;
            eventModel.Tags.Clear();
            foreach (var tagId in request.TagIds)
            {
                var tag = await _tagRepository.GetTagByIdAsync(tagId);
                if (tag != null)
                {
                    eventModel.Tags.Add(tag);
                }
            }

            await _repository.UpdateAsync(eventModel);
            return await GetEventByIdAsync(eventId, userId);
        }

        public async Task DeleteEventAsync(Guid eventId, Guid userId)
        {
            var eventModel = await _repository.GetByIdAsync(eventId);
            if (eventModel == null)
            {
                throw new Exception("Event not found");
            }

            if (eventModel.OrganizerId != userId)
            {
                throw new Exception("You can only delete events you organized");
            }

            await _repository.DeleteAsync(eventId);
        }

        public async Task JoinEventAsync(Guid eventId, Guid userId)
        {
            var eventModel = await _repository.GetByIdAsync(eventId);
            if (eventModel == null)
            {
                throw new Exception("Event not found");
            }

            if (!eventModel.IsPublic)
            {
                throw new Exception("This is a private event");
            }

            var participantCount = eventModel.Participants.Count;
            if (eventModel.Capacity > 0 && participantCount >= eventModel.Capacity)
            {
                throw new Exception("Event is full");
            }

            if (eventModel.Participants.Any(p => p.Id == userId))
            {
                throw new Exception("You are already participating in this event");
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            eventModel.Participants.Add(user);
            await _repository.UpdateAsync(eventModel);
        }

        public async Task LeaveEventAsync(Guid eventId, Guid userId)
        {
            var eventModel = await _repository.GetByIdAsync(eventId);
            if (eventModel == null)
            {
                throw new Exception("Event not found");
            }

            var participant = eventModel.Participants.FirstOrDefault(p => p.Id == userId);
            if (participant == null)
            {
                throw new Exception("You are not participating in this event");
            }

            eventModel.Participants.Remove(participant);
            await _repository.UpdateAsync(eventModel);
        }

        public async Task<ICollection<CalendarEventResponse>> GetUserEventsAsync(Guid userId)
        {
            var events = await _repository.GetUserEventsAsync(userId);
            var calendarEvents = new List<CalendarEventResponse>();

            foreach (var eventModel in events)
            {
                calendarEvents.Add(new CalendarEventResponse
                {
                    Id = eventModel.Id,
                    Title = eventModel.Title,
                    Date = eventModel.Date,
                    Color = eventModel.Tags.First().Color
                });
            }

            return calendarEvents;
        }

        private static string? GetShortDescription(string? description)
        {
            if (string.IsNullOrEmpty(description)) return null;
            return description.Length > 100 ? description[..100] + "..." : description;
        }

    }
}