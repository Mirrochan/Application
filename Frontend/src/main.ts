import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideStore } from '@ngrx/store';
import { provideEffects } from '@ngrx/effects';
import { provideStoreDevtools } from '@ngrx/store-devtools';
import { routes } from './app/app.routes';
import { eventListReducer } from './app/state/event-list/event-list.reducer';
import { EventsEffects } from './app/state/event-list/event-list.effects';
import { isDevMode } from '@angular/core';

bootstrapApplication(AppComponent, {
  providers: [
    provideRouter(routes),
    provideHttpClient(),
    provideStore({ 
      events: eventListReducer 
    }),
    provideEffects([EventsEffects]),
    provideStoreDevtools({ 
      maxAge: 25, 
      logOnly: !isDevMode(),
      connectInZone: true
    })
  ]
}).catch(err => console.error(err));