import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { EventsService } from '../../../data/services/events.service';
import { CreateEventRequest } from '../../../data/interfaces/event.model';

@Component({
  selector: 'app-create-event',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './create-event.component.html',
  styleUrls: ['./create-event.component.scss']
})
export class CreateEventComponent {
  eventForm: FormGroup;
  submitting = false;
  backendError: string | null = null;
  constructor(
    private fb: FormBuilder,
    private router: Router,
    private eventService: EventsService
  ) {
    this.eventForm = this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required],
      date: ['', Validators.required],
      time: ['', Validators.required],
      location: ['', Validators.required],
      capacity: [''],
      visibility: [true, Validators.required],
    });
  }

  onSubmit() {
    this.eventForm.markAllAsTouched(); if (this.eventForm.invalid) return;
    this.submitting = true;

    const form = this.eventForm.value;
    const dateConst = this.combineDateTime(form.date, form.time);

    const newEvent: CreateEventRequest = {
      title: form.title,
      description: form.description,
      date: dateConst,

      location: form.location,
      capacity: form.capacity ? +form.capacity : 0,
      isPublic: form.visibility
    };

    this.eventService.createEvent(newEvent).subscribe({
      next: (data1) => {
        this.submitting = false;
        this.backendError = null;
        this.router.navigate(['/event-details', data1.id]);
      },
      error: (err) => {
        this.submitting = false;
        console.error('Full error object:', err);

        const rawMessage =
          err.error?.error ||
          err.error?.title ||
          err.message;

        this.backendError = this.cleanValidationMessage(rawMessage);
      }

    });
  }
  onCancel() {
    this.router.navigate(['/events']);
  }

  private combineDateTime(date: string, time: string): Date {
    const dateTime = new Date(`${date}T${time}`);
    return dateTime;
  }


  cleanValidationMessage(input: any): string {
    if (!input || typeof input !== 'string') return '';

    const lines = input.replace(/\r/g, '').split('\n');

    const filtered = lines.filter(line =>
      !line.includes('Validation failed') &&
      !line.includes('Severity:') &&
      !line.trim().startsWith('--')
    );

    if (filtered.length === 0) {
      const match = input.match(/--\s*(.*?):\s*(.*?)\s*Severity/i);
      if (match) {
        return `${match[1]}: ${match[2]}`;
      }
    }

    return filtered.join('\n').trim();
  }

}
