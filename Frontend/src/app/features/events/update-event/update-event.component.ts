import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { EventsService } from '../../../data/services/events.service';
import { EventFullInfo, UpdateEventRequest } from '../../../data/interfaces/event.model';
import { Tag } from '../../../data/interfaces/tag.model';
import { TagService } from '../../../data/services/tag.service';

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

  availableTags: Tag[] = [];
  selectedTag: Tag[] = [];

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private eventService: EventsService,
    private route: ActivatedRoute,
    private tagService: TagService
  ) { }

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

  private fetchAvailableTags(): void {

    this.tagService.getAllTags().subscribe({
      next: (tags: any) => {
        this.availableTags = tags;
        this.availableTags = this.availableTags.filter(
          tag => !this.selectedTag.some(selected => selected.id === tag.id)
        );
      },
      error: (err) => {
        console.error('Error fetching tags:', err);
      }
    });
  }
  public selectTag(tag: Tag): void {
    if(this.selectedTag.length == 5) return;
    this.selectedTag.push(tag);
    this.availableTags = this.availableTags.filter(t => t.id !== tag.id);
  }
  public deselectTag(tag: Tag): void {
    this.availableTags.push(tag);
    this.selectedTag = this.selectedTag.filter(t => t.id !== tag.id);
  }
  private loadEventData(): void {
    this.eventService.getEventById(this.eventId).subscribe({
      next: (data) => {
        this.eventData = data;
        this.selectedTag = this.eventData.tags;
        this.fetchAvailableTags();
        const dateObj = new Date(this.eventData.date);
        const date = dateObj.toISOString().split('T')[0];
        const time = dateObj.toTimeString().substring(0, 5);

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
      isPublic: form.visibility,
      tagIds: this.selectedTag.map(tag => tag.id)
    };

    this.eventService.updateEvent(this.eventId, updatedEvent).subscribe({
      next: (res) => {
        this.submitting = false;
        this.backendError = null;
        this.router.navigate(['/event-details', this.eventId]);
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

  onCancel(): void {
    this.router.navigate(['/events']);
  }


  private combineDateTime(date: string, time: string): Date {
    return new Date(`${date}T${time}`);
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
