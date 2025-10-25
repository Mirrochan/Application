import { Component, OnInit } from '@angular/core';
import { EventsService } from '../../../data/services/events.service';
import { AuthService } from '../../../data/services/auth.service';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EventModel } from '../../../data/interfaces/event.model';

@Component({
  selector: 'app-event-list',
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './event-list.component.html',
  styleUrl: './event-list.component.scss'
})
export class EventsListComponent implements OnInit {


  events: EventModel[] = [];
  searchTerm = '';
  isLoading = true;

  constructor(
    private eventsService: EventsService,
    public authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadEvents();
  }

  loadEvents(): void {
    this.eventsService.getPublicEvents().subscribe({
      next: (events) => {

        this.events = events;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading events:', error);
        this.isLoading = false;
      }
    });
  }

  joinEvent(event: EventModel): void {
    this.eventsService.joinEvent(event.id).subscribe({
      next: () => {
        event.isParticipant = true;
        event.participantCount++;
      },
      error: (error) => {
        console.error('Error joining event:', error);
        alert(error.error?.error || 'Failed to join event');
      }
    });
  }

  leaveEvent(event: EventModel): void {
    this.eventsService.leaveEvent(event.id).subscribe({
      next: () => {
        event.isParticipant = false;
        event.participantCount--;
      },
      error: (error) => {
        console.error('Error leaving event:', error);
        alert(error.error?.error || 'Failed to leave event');
      }
    });
  }


  get filteredEvents(): EventModel[] {
    if (!this.searchTerm) return this.events;

    const searchLower = this.searchTerm.toLowerCase();
    return this.events.filter(event =>
      event.title.toLowerCase().includes(searchLower) ||
      event.shortDescription.toLowerCase().includes(searchLower) ||
      event.location.toLowerCase().includes(searchLower)
    );
  }

  viewEventDetails(eventId: string): void {
    this.router.navigate(['/events', eventId]);
  }

  openEvent(id: string) {
    this.router.navigate(['/event-details', id]);
  }
}