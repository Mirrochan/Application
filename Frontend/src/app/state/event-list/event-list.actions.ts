import { createAction, props } from '@ngrx/store';
import { EventModel } from '../../data/interfaces/event.model';
import { Tag } from '../../data/interfaces/tag.model';


export const loadEvents = createAction('[Events] Load Events');
export const loadEventsSuccess = createAction(
  '[Events] Load Events Success',
  props<{ events: EventModel[] }>()
);
export const loadEventsFailure = createAction(
  '[Events] Load Events Failure',
  props<{ error: string }>()
);

export const joinEvent = createAction(
  '[Events] Join Event',
  props<{ eventId: string }>()
);
export const joinEventSuccess = createAction(
  '[Events] Join Event Success',
  props<{ eventId: string }>()
);
export const joinEventFailure = createAction(
  '[Events] Join Event Failure',
  props<{ error: string }>()
);

export const leaveEvent = createAction(
  '[Events] Leave Event',
  props<{ eventId: string }>()
);
export const leaveEventSuccess = createAction(
  '[Events] Leave Event Success',
  props<{ eventId: string }>()
);
export const leaveEventFailure = createAction(
  '[Events] Leave Event Failure',
  props<{ error: string }>()
);


export const filterEventsBySearch = createAction(
  '[Events] Filter Events By Search',
  props<{ searchTerm: string }>()
);

export const filterEventsByTags = createAction(
  '[Events] Filter Events By Tags',
  props<{ tagIds: string[] }>()
);


export const loadTags = createAction('[Tags] Load Tags');
export const loadTagsSuccess = createAction(
  '[Tags] Load Tags Success',
  props<{ tags: Tag[] }>()
);
export const loadTagsFailure = createAction(
  '[Tags] Load Tags Failure',
  props<{ error: string }>()
);