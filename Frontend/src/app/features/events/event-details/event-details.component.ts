import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { EventsService } from '../../../data/services/events.service';
import { AuthService } from '../../../data/services/auth.service';

interface EventFullInfo {
  id: string;
  title: string;
  description: string;
  date: Date;
  location: string;
  capacity?: number;
  participants: string[];
  isOrganizer: boolean;
  isParticipant: boolean;
}

@Component({
  selector: 'app-event-details',
  imports: [CommonModule, RouterModule],
  templateUrl: './event-details.component.html',
  styleUrl: './event-details.component.scss'
})
export class EventDetailsComponent implements OnInit {
  event?: EventFullInfo;
  isLoading = true;
  error: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private eventsService: EventsService,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadEventDetails();
  }

  loadEventDetails(): void {
    const eventId = this.route.snapshot.paramMap.get('id');
    
    if (!eventId) {
      this.error = 'Event ID not provided';
      this.isLoading = false;
      return;
    }

    this.eventsService.getEventById(eventId).subscribe({
      next: (event: any) => {
        this.event = event;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading event details:', error);
        this.error = error.error?.error || 'Failed to load event details';
        this.isLoading = false;
      }
    });
  }

  joinEvent(): void {
    if (!this.event) return;

    this.eventsService.joinEvent(this.event.id).subscribe({
      next: () => {
        if (this.event) {
          this.event.isParticipant = true;
          this.loadEventDetails();
        }
      },
      error: (error) => {
        console.error('Error joining event:', error);
        alert(error.error?.error || 'Failed to join event');
      }
    });
  }

  leaveEvent(): void {
    if (!this.event) return;

    this.eventsService.leaveEvent(this.event.id).subscribe({
      next: () => {
        if (this.event) {
          this.event.isParticipant = false;
          this.loadEventDetails();
        }
      },
      error: (error) => {
        console.error('Error leaving event:', error);
        alert(error.error?.error || 'Failed to leave event');
      }
    });
  }

  editEvent(): void {
    if (this.event) {
      this.router.navigate(['/update-event', this.event.id, ]);
    }
  }

  deleteEvent(): void {
    if (!this.event) return;

    const confirmed = confirm('Are you sure you want to delete this event? This action cannot be undone.');
    
    if (confirmed) {
      this.eventsService.deleteEvent(this.event.id).subscribe({
        next: () => {
          this.router.navigate(['/events']);
        },
        error: (error) => {
          console.error('Error deleting event:', error);
          alert(error.error?.error || 'Failed to delete event');
        }
      });
    }
  }

  isEventFull(): boolean {
    if (!this.event?.capacity) return false;
    return this.event.participants.length >= this.event.capacity;
  }



  getButtonClasses(): string {
    if (this.event?.isParticipant) {
      return 'bg-red-600 text-white hover:bg-red-700';
    } else if (this.isEventFull()) {
      return 'bg-gray-400 text-white cursor-not-allowed';
    } else {
      return 'bg-green-600 text-white hover:bg-green-700';
    }
  }
    goBack(): void {
      this.router.navigate(["/events"]);
  }
}