import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateEventRequest, EventFullInfo, EventModel, MyEvents, UpdateEventRequest } from '../interfaces/event.model';

@Injectable({
  providedIn: 'root'
})
export class EventsService {
  private readonly API_URL = 'http://localhost:5107';

  constructor(private http: HttpClient) { }

  getPublicEvents(): Observable<EventModel[]> {
    return this.http.get<EventModel[]>(`${this.API_URL}/events`, {
      withCredentials: true
    });
  }

  getEventById(id: string): Observable<EventFullInfo> {
    return this.http.get<EventFullInfo>(`${this.API_URL}/events/${id}`, {
      withCredentials: true
    });
  }

  createEvent(eventData: CreateEventRequest): Observable<EventModel> {
    return this.http.post<EventModel>(`${this.API_URL}/events`, eventData, {
      withCredentials: true
    });
  }

  joinEvent(id: string): Observable<any> {
    return this.http.post(`${this.API_URL}/events/${id}/join`, {}, {
      withCredentials: true
    });
  }

  leaveEvent(id: string): Observable<any> {
    return this.http.post(`${this.API_URL}/events/${id}/leave`, {}, {
      withCredentials: true
    });
  }

  getUserEvents(): Observable<MyEvents[]> {
    return this.http.get<MyEvents[]>(`${this.API_URL}/users/me/events`, {
      withCredentials: true
    });
  }

  deleteEvent(id:string):Observable<any>{
    return this.http.delete(`${this.API_URL}/events/${id}`, {
      withCredentials: true
    })};
  updateEvent(id: string, data: UpdateEventRequest): Observable<any> {
  return this.http.patch(`${this.API_URL}/events/${id}`, data, {
    withCredentials: true
  });
}

}