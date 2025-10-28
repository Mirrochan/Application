import { Tag } from "./tag.model";
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
  tags: Tag[];
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
  tags: Tag[]
}
export interface MyEvents 
{
  id: string;
  title: string;
  date: Date;
  color: string;
}
export interface CreateEventRequest {
  title: string;
  description: string;
  date: Date;
  location: string;
  capacity?: number;
  isPublic: boolean;
  tagIds: string[];
}

export interface UpdateEventRequest {
  title?: string;
  description?: string;
  date?: Date;
  location?: string;
  capacity?: number;
  isPublic:boolean;
  tagIds: string[];

}