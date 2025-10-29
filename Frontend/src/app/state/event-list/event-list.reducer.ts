import { createReducer, on } from '@ngrx/store';
import * as EventsActions from './event-list.actions'; 
import { initialState } from './event-list.state';
import { EventModel } from '../../data/interfaces/event.model';
import { Tag } from '../../data/interfaces/tag.model';

export const eventListReducer = createReducer(
  initialState,
  
  on(EventsActions.loadEvents, (state) => ({
    ...state,
    loading: true,
    error: null
  })),
  on(EventsActions.loadEventsSuccess, (state, { events }) => ({
    ...state,
    events,
    filteredEvents: events,
    loading: false
  })),
  on(EventsActions.loadEventsFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),

  on(EventsActions.loadTagsSuccess, (state, { tags }) => ({
    ...state,
    tags
  })),


  on(EventsActions.filterEventsBySearch, (state, { searchTerm }) => {
    const filteredEvents = filterEvents(state.events, searchTerm, state.selectedTagIds, state.tags);
    return {
      ...state,
      searchTerm,
      filteredEvents
    };
  }),

  on(EventsActions.filterEventsByTags, (state, { tagIds }) => {
    const filteredEvents = filterEvents(state.events, state.searchTerm, tagIds, state.tags);
    return {
      ...state,
      selectedTagIds: tagIds,
      filteredEvents
    };
  }),


  on(EventsActions.joinEventSuccess, (state, { eventId }) => ({
    ...state,
    events: state.events.map(event => 
      event.id === eventId 
        ? { 
            ...event, 
            isParticipant: true, 
            participantCount: event.participantCount + 1 
          }
        : event
    ),
    filteredEvents: state.filteredEvents.map(event => 
      event.id === eventId 
        ? { 
            ...event, 
            isParticipant: true, 
            participantCount: event.participantCount + 1 
          }
        : event
    )
  })),

  on(EventsActions.leaveEventSuccess, (state, { eventId }) => ({
    ...state,
    events: state.events.map(event => 
      event.id === eventId 
        ? { 
            ...event, 
            isParticipant: false, 
            participantCount: event.participantCount - 1 
          }
        : event
    ),
    filteredEvents: state.filteredEvents.map(event => 
      event.id === eventId 
        ? { 
            ...event, 
            isParticipant: false, 
            participantCount: event.participantCount - 1 
          }
        : event
    )
  }))
);


function filterEvents(
  events: EventModel[], 
  searchTerm: string, 
  selectedTagIds: string[], 
  tags: Tag[]
): EventModel[] {
  let filtered = events;

 
  if (searchTerm) {
    const searchLower = searchTerm.toLowerCase();
    filtered = filtered.filter(event =>
      event.title.toLowerCase().includes(searchLower) ||
      event.shortDescription.toLowerCase().includes(searchLower) ||
      event.location.toLowerCase().includes(searchLower)
    );
  }

  if (selectedTagIds.length > 0) {
    filtered = filtered.filter(event =>
      event.tags.some(tag => selectedTagIds.includes(tag.id))
    );
  }

  return filtered;
}