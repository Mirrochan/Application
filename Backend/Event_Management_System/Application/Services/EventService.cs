using Application.Abstractions;
using Application.DTOs;
using Application.DTOs.Application.DTOs;
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

        public EventService(
            IEventRepository repository,
            IUserRepository userRepository,
            IValidator<CreateEventRequest> createEventValidator,
            IValidator<UpdateEventRequest> updateEventValidator)
        {
            _repository = repository;
            _userRepository = userRepository;
            _createEventValidator = createEventValidator;
            _updateEventValidator = updateEventValidator;
        }

        public async Task<ICollection<EventSummaryResponse>> GetPublicEventsAsync(Guid? userId = null)
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
                    StartAt = eventModel.StartAt,
                    EndAt = eventModel.EndAt,
                    Location = eventModel.Location,
                    Capacity = eventModel.Capacity,
                    IsPublic = eventModel.IsPublic,
                    IsFull = isFull,
                    ParticipantCount = participantCount,
                    OrganizerName = $"{eventModel.Organizer.FirstName} {eventModel.Organizer.LastName}",
                    IsParticipant = isParticipant
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
            var availableSpots = eventModel.Capacity == 0 ? int.MaxValue : eventModel.Capacity - participantCount;
            var isOrganizer = userId.HasValue && eventModel.OrganizerId == userId.Value;
            var isParticipant = userId.HasValue && eventModel.Participants.Any(p => p.Id == userId.Value);

            var organizerResponse = new UserResponse
            {
                Id = eventModel.Organizer.Id,
                Email = eventModel.Organizer.Email,
                FirstName = eventModel.Organizer.FirstName,
                LastName = eventModel.Organizer.LastName
            };

            var participantResponses = eventModel.Participants.Select(p => new UserResponse
            {
                Id = p.Id,
                Email = p.Email,
                FirstName = p.FirstName,
                LastName = p.LastName
            }).ToList();

            return new EventResponse
            {
                Id = eventModel.Id,
                Title = eventModel.Title,
                Description = eventModel.Description,
                StartAt = eventModel.StartAt,
                EndAt = eventModel.EndAt,
                Location = eventModel.Location,
                Capacity = eventModel.Capacity,
                IsPublic = eventModel.IsPublic,
                IsFull = isFull,
                ParticipantCount = participantCount,
                Organizer = organizerResponse,
                Participants = participantResponses,
                IsOrganizer = isOrganizer,
                IsParticipant = isParticipant
            };
        }

        public async Task<EventResponse> CreateEventAsync(CreateEventRequest request, Guid organizerId)
        {
            // Validate request
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

            var eventModel = new EventModel
            {
                Id = Guid.NewGuid(),
                Title = request.Title.Trim(),
                Description = request.Description?.Trim(),
                StartAt = request.StartAt,
                EndAt = request.EndAt,
                Location = request.Location.Trim(),
                Capacity = request.Capacity,
                IsPublic = request.IsPublic,
                OrganizerId = organizerId
            };

            await _repository.AddAsync(eventModel);
            return await GetEventByIdAsync(eventModel.Id, organizerId);
        }

        public async Task<EventResponse> UpdateEventAsync(Guid eventId, UpdateEventRequest request, Guid userId)
        {
            // Validate request
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

            if (request.StartAt.HasValue)
                eventModel.StartAt = request.StartAt.Value;

            if (request.EndAt.HasValue)
                eventModel.EndAt = request.EndAt.Value;

            if (!string.IsNullOrWhiteSpace(request.Location))
                eventModel.Location = request.Location.Trim();

            if (request.Capacity.HasValue)
                eventModel.Capacity = request.Capacity.Value;

            if (request.IsPublic.HasValue)
                eventModel.IsPublic = request.IsPublic.Value;


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
                var eventType = eventModel.OrganizerId == userId ? "organized" : "participating";
                calendarEvents.Add(new CalendarEventResponse
                {
                    Id = eventModel.Id,
                    Title = eventModel.Title,
                    StartAt = eventModel.StartAt,
                    EndAt = eventModel.EndAt,
                    Location = eventModel.Location,
                    EventType = eventType
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