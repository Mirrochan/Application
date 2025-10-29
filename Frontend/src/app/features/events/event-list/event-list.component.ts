import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { Store } from '@ngrx/store';
import { Subscription } from 'rxjs';


import { AuthService } from '../../../data/services/auth.service';

import { EventModel } from '../../../data/interfaces/event.model';
import { Tag } from '../../../data/interfaces/tag.model';


import { 
  loadEvents, 
  joinEvent, 
  leaveEvent,
  filterEventsBySearch,
  filterEventsByTags,
  loadTags
} from '../../../state/event-list/event-list.actions';
import { 
  selectFilteredEvents, 
  selectTags,
  selectedTagIds,
  selectSearchTerm,
  selectEventsLoading,
  selectEventsError 
} from '../../../state/event-list/event-list.selectors';
import { AppState } from '../../../state/app.state';

@Component({
  selector: 'app-event-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './event-list.component.html',
  styleUrl: './event-list.component.scss'
})
export class EventsListComponent implements OnInit, OnDestroy {
  filteredEvents: EventModel[] = [];
  tags: Tag[] = [];
  selectedTagIds: string[] = [];
  searchTerm = '';
  isLoading = false;
  error: string | null = null;

 
  isDropdownOpen = false;
  tagSearchTerm = '';
  filteredTags: Tag[] = [];
  
  private subscriptions = new Subscription();

  constructor(
    private store: Store<AppState>,
    public authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
  
    this.store.dispatch(loadEvents());
    this.store.dispatch(loadTags());

  
    this.subscriptions.add(
      this.store.select(selectFilteredEvents).subscribe(events => {
        this.filteredEvents = events;
      })
    );

    this.subscriptions.add(
      this.store.select(selectTags).subscribe(tags => {
        this.tags = tags;
        this.filteredTags = tags; 
      })
    );

    this.subscriptions.add(
      this.store.select(selectedTagIds).subscribe(tagIds => {
        this.selectedTagIds = tagIds;
      })
    );

    this.subscriptions.add(
      this.store.select(selectSearchTerm).subscribe(term => {
        this.searchTerm = term;
      })
    );

    this.subscriptions.add(
      this.store.select(selectEventsLoading).subscribe(loading => {
        this.isLoading = loading;
      })
    );

    this.subscriptions.add(
      this.store.select(selectEventsError).subscribe(error => {
        this.error = error;
      })
    );
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }


  onSearchChange(): void {
    this.store.dispatch(filterEventsBySearch({ searchTerm: this.searchTerm }));
  }


  toggleDropdown(): void {
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  closeDropdown(): void {
    this.isDropdownOpen = false;
  }

  onTagSearch(): void {
    if (!this.tagSearchTerm) {
      this.filteredTags = this.tags;
    } else {
      const searchLower = this.tagSearchTerm.toLowerCase();
      this.filteredTags = this.tags.filter(tag => 
        tag.name.toLowerCase().includes(searchLower)
      );
    }
  }

  toggleTagFilter(tagId: string): void {
    const newTagIds = this.selectedTagIds.includes(tagId)
      ? this.selectedTagIds.filter(id => id !== tagId)
      : [...this.selectedTagIds, tagId];
    
    this.store.dispatch(filterEventsByTags({ tagIds: newTagIds }));
  }

  isTagSelected(tagId: string): boolean {
    return this.selectedTagIds.includes(tagId);
  }

  removeTag(tagId: string): void {
    const newTagIds = this.selectedTagIds.filter(id => id !== tagId);
    this.store.dispatch(filterEventsByTags({ tagIds: newTagIds }));
  }

  clearTagFilters(): void {
    this.store.dispatch(filterEventsByTags({ tagIds: [] }));
  }

  selectAllTags(): void {
    const allTagIds = this.tags.map(tag => tag.id);
    this.store.dispatch(filterEventsByTags({ tagIds: allTagIds }));
  }

  getTagById(tagId: string): Tag | undefined {
    return this.tags.find(tag => tag.id === tagId);
  }


  joinEvent(event: EventModel): void {
    this.store.dispatch(joinEvent({ eventId: event.id }));
  }

  leaveEvent(event: EventModel): void {
    this.store.dispatch(leaveEvent({ eventId: event.id }));
  }

  openEvent(id: string): void {
    this.router.navigate(['/event-details', id]);
  }
}