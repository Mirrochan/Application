
import { EventsState } from "./event-list/event-list.state";

export interface AppState {
  events: EventsState;
}

export interface RootState {
  events: EventsState;
}