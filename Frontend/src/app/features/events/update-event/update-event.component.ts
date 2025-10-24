import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { EventsService } from '../../../data/services/events.service';
import { EventFullInfo, UpdateEventRequest } from '../../../data/interfaces/event.model';

@Component({
  selector: 'app-update-event',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './update-event.component.html',
  styleUrls: ['./update-event.component.scss']
})
export class UpdateEventComponent implements OnInit {
  eventForm!: FormGroup;
  submitting = false;
  backendError: string | null = null;
  eventId!: string;
  eventData!: EventFullInfo;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private eventService: EventsService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.eventId = this.route.snapshot.paramMap.get('id')!;
    this.initializeForm();
    this.loadEventData();
  }

  private initializeForm(): void {
    this.eventForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(3)]],
      description: ['', Validators.required],
      date: ['', Validators.required],
      time: ['', Validators.required],
      location: ['', Validators.required],
      capacity: [''],
      visibility: [true, Validators.required]
    });
  }

  private loadEventData(): void {
    this.eventService.getEventById(this.eventId).subscribe({
      next: (data) => {
        this.eventData = data;

        // Розділяємо дату й час для форми
        const dateObj = new Date(this.eventData.date);
        const date = dateObj.toISOString().split('T')[0];
        const time = dateObj.toTimeString().substring(0, 5);

        // Заповнюємо форму отриманими даними
        this.eventForm.patchValue({
          title: this.eventData.title,
          description: this.eventData.description,
          date: date,
          time: time,
          location: this.eventData.location,
          capacity: this.eventData['capacity'] || ''
        });
      },
      error: (err) => {
        console.error('Error loading event:', err);
        this.backendError = 'Failed to load event details.';
      }
    });
  }

  onSubmit(): void {
    this.eventForm.markAllAsTouched();
    if (this.eventForm.invalid) return;

    this.submitting = true;

    const form = this.eventForm.value;
    const dateConst = this.combineDateTime(form.date, form.time);

    const updatedEvent: UpdateEventRequest = {
      title: form.title,
      description: form.description,
      date: dateConst,
      location: form.location,
      capacity: form.capacity ? +form.capacity : 0,
      isPublic: form.visibility
    };

    this.eventService.updateEvent(this.eventId, updatedEvent).subscribe({
      next: (res) => {
        this.submitting = false;
        this.backendError = null;
        this.router.navigate(['/event-details', this.eventId]);
      },
      error: (err) => {
        this.submitting = false;
        console.error('Error updating event:', err);
        this.backendError = err.error?.error || 'Unknown error occurred.';
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/events']);
  }

  private combineDateTime(date: string, time: string): Date {
    return new Date(`${date}T${time}`);
  }
}
