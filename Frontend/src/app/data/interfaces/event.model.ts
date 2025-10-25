import { User } from "./user.model";


export interface EventModel {
  id: string;
  title: string;
  shortDescription: string;
  date: Date
  location: string;
  capacity?: number;
  isPublic: boolean;
  isFull:boolean;
  participantCount: number;
  organizerName: string;
  isParticipant?: boolean;
}
export interface EventFullInfo{
  id: string;
  title: string;
  description: string;
  date: Date
  location: string;
  capacity?: number;
  participants: string[];
  isOrganizer: boolean;
  isParticipant: boolean;
}
export interface MyEvents 
{
  id: string;
  title: string;
  date: Date
}
export interface CreateEventRequest {
  title: string;
  description: string;
  date: Date;
  location: string;
  capacity?: number;
  isPublic: boolean;
}

export interface UpdateEventRequest {
  title?: string;
  description?: string;
  date?: Date;
  location?: string;
  capacity?: number;
  isPublic:boolean;
}