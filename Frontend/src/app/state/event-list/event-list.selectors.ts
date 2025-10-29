import { createSelector, createFeatureSelector } from '@ngrx/store';
import { EventsState } from './event-list.state';

export const selectEventsState = createFeatureSelector<EventsState>('events');

export const selectEvents = createSelector(
  selectEventsState,
  (state: EventsState) => state.events
);

export const selectFilteredEvents = createSelector(
  selectEventsState,
  (state: EventsState) => state.filteredEvents
);

export const selectTags = createSelector(
  selectEventsState,
  (state: EventsState) => state.tags
);

export const selectedTagIds = createSelector(
  selectEventsState,
  (state: EventsState) => state.selectedTagIds
);

export const selectSearchTerm = createSelector(
  selectEventsState,
  (state: EventsState) => state.searchTerm
);

export const selectEventsLoading = createSelector(
  selectEventsState,
  (state: EventsState) => state.loading
);

export const selectEventsError = createSelector(
  selectEventsState,
  (state: EventsState) => state.error
);