import { Component } from '@angular/core';
import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { EventsListComponent } from './features/events/event-list/event-list.component';
import { AuthGuard } from './Guards/auth.guard';
import { MyEventsComponent } from './features/events/my-events/my-events.component';
import { EventDetailsComponent } from './features/events/event-details/event-details.component';
import { CreateEventComponent } from './features/events/create-event/create-event.component';
import { UpdateEventComponent } from './features/events/update-event/update-event.component';
import { AiAssistantComponent } from './features/ai/ai-assistant/ai-assistant.component';

export const routes: Routes = [
    {path:'', component:LoginComponent } ,
    {path:'login', component:LoginComponent } ,
    {path:'register', component:RegisterComponent },
    {path:'events', component: EventsListComponent, canActivate: [AuthGuard]} ,
    {path:'my-events', component: MyEventsComponent, canActivate: [AuthGuard]},
    {path:'event-details/:id', component: EventDetailsComponent, canActivate: [AuthGuard]},
    {path:'create-event', component:CreateEventComponent, canActivate: [AuthGuard]},
    {path:'update-event/:id', component:UpdateEventComponent, canActivate: [AuthGuard]},
    {path:'ai-assistant', component:AiAssistantComponent, canActivate: [AuthGuard]},
];
