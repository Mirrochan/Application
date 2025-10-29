import { EventModel } from '../../data/interfaces/event.model';
import { Tag } from '../../data/interfaces/tag.model';

export interface EventsState {
  events: EventModel[];
  filteredEvents: EventModel[];
  tags: Tag[];
  searchTerm: string;
  selectedTagIds: string[];
  loading: boolean;
  error: string | null;
}

export const initialState: EventsState = {
  events: [],
  filteredEvents: [],
  tags: [],
  searchTerm: '',
  selectedTagIds: [],
  loading: false,
  error: null
};