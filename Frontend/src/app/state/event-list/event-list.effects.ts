import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { map, mergeMap, catchError } from 'rxjs/operators';
import { EventsService } from '../../data/services/events.service';
import { TagService } from '../../data/services/tag.service';
import * as EventsActions from './event-list.actions';

@Injectable()
export class EventsEffects {
  private actions$ = inject(Actions);
  private eventsService = inject(EventsService);
  private tagService = inject(TagService);

  loadEvents$ = createEffect(() =>
    this.actions$.pipe(
      ofType(EventsActions.loadEvents),
      mergeMap(() =>
        this.eventsService.getPublicEvents().pipe(
          map(events => EventsActions.loadEventsSuccess({ events })),
          catchError(error => of(EventsActions.loadEventsFailure({ error: error.message })))
        )
      )
    )
  );

  loadTags$ = createEffect(() =>
    this.actions$.pipe(
      ofType(EventsActions.loadTags),
      mergeMap(() =>
        this.tagService.getAllTags().pipe(
          map(tags => EventsActions.loadTagsSuccess({ tags })),
          catchError(error => of(EventsActions.loadTagsFailure({ error: error.message })))
        )
      )
    )
  );

  joinEvent$ = createEffect(() =>
    this.actions$.pipe(
      ofType(EventsActions.joinEvent),
      mergeMap(({ eventId }) =>
        this.eventsService.joinEvent(eventId).pipe(
          map(() => EventsActions.joinEventSuccess({ eventId })),
          catchError(error => of(EventsActions.joinEventFailure({ error: error.message })))
        )
      )
    )
  );

  leaveEvent$ = createEffect(() =>
    this.actions$.pipe(
      ofType(EventsActions.leaveEvent),
      mergeMap(({ eventId }) =>
        this.eventsService.leaveEvent(eventId).pipe(
          map(() => EventsActions.leaveEventSuccess({ eventId })),
          catchError(error => of(EventsActions.leaveEventFailure({ error: error.message })))
        )
      )
    )
  );

  constructor() {
    console.log('EventsEffects created', {
      hasEventsService: !!this.eventsService,
      hasTagService: !!this.tagService,
      hasActions: !!this.actions$
    });
  }
}