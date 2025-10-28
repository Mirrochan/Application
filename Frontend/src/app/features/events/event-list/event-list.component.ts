import { Component, OnInit, HostListener } from '@angular/core';
import { EventsService } from '../../../data/services/events.service';
import { AuthService } from '../../../data/services/auth.service';
import { TagService } from '../../../data/services/tag.service';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EventModel } from '../../../data/interfaces/event.model';
import { Tag } from '../../../data/interfaces/tag.model';

@Component({
  selector: 'app-event-list',
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './event-list.component.html',
  styleUrl: './event-list.component.scss'
})
export class EventsListComponent implements OnInit {
  events: EventModel[] = [];
  allTags: Tag[] = [];
  filteredTags: Tag[] = [];
  selectedTagIds: string[] = [];
  searchTerm = '';
  tagSearchTerm = '';
  isLoading = true;
  isTagsLoading = true;
  isDropdownOpen = false;

  constructor(
    private eventsService: EventsService,
    private tagService: TagService,
    public authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadEvents();
    this.loadTags();
  }

  @HostListener('document:keydown.escape')
  onEscapePress() {
    this.closeDropdown();
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

  loadTags(): void {
    this.tagService.getAllTags().subscribe({
      next: (tags: any) => {
        this.allTags = tags;
        this.filteredTags = tags;
        this.isTagsLoading = false;
      },
      error: (error) => {
        console.error('Error loading tags:', error);
        this.isTagsLoading = false;
      }
    });
  }

  toggleDropdown(): void {
    this.isDropdownOpen = !this.isDropdownOpen;
    if (this.isDropdownOpen) {
      this.tagSearchTerm = '';
      this.filteredTags = this.allTags;
    }
  }

  closeDropdown(): void {
    this.isDropdownOpen = false;
  }

  onTagSearch(): void {
    if (!this.tagSearchTerm.trim()) {
      this.filteredTags = this.allTags;
      return;
    }

    const searchLower = this.tagSearchTerm.toLowerCase();
    this.filteredTags = this.allTags.filter(tag =>
      tag.name.toLowerCase().includes(searchLower)
    );
  }

  toggleTagFilter(tagId: string): void {
    const index = this.selectedTagIds.indexOf(tagId);
    if (index > -1) {
      this.selectedTagIds.splice(index, 1);
    } else {
      this.selectedTagIds.push(tagId);
    }
  }

  removeTag(tagId: string): void {
    this.selectedTagIds = this.selectedTagIds.filter(id => id !== tagId);
  }

  selectAllTags(): void {
    this.selectedTagIds = this.allTags.map(tag => tag.id);
  }

  isTagSelected(tagId: string): boolean {
    return this.selectedTagIds.includes(tagId);
  }

  clearTagFilters(): void {
    this.selectedTagIds = [];
    this.closeDropdown();
  }

  getTagById(tagId: string): Tag | undefined {
    return this.allTags.find(tag => tag.id === tagId);
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
    let filtered = this.events;

    if (this.searchTerm) {
      const searchLower = this.searchTerm.toLowerCase();
      filtered = filtered.filter(event =>
        event.title.toLowerCase().includes(searchLower) ||
        event.shortDescription.toLowerCase().includes(searchLower) ||
        event.location.toLowerCase().includes(searchLower)
      );
    }

 
    if (this.selectedTagIds.length > 0) {
      filtered = filtered.filter(event =>
        this.selectedTagIds.every(selectedTagId =>
          event.tags.some(eventTag => eventTag.id === selectedTagId)
        )
      );
    }

    return filtered;
  }

  viewEventDetails(eventId: string): void {
    this.router.navigate(['/events', eventId]);
  }

  openEvent(id: string) {
    this.router.navigate(['/event-details', id]);
  }
}