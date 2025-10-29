import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Tag } from "../interfaces/tag.model";

@Injectable({
  providedIn: 'root'
})
export class TagService {
  private readonly API_URL = 'http://localhost:5107';

  constructor(private http: HttpClient) { }
  getAllTags() {
    return this.http.get<Tag[]>(`${this.API_URL}/tags/getAllTags`, {
      withCredentials: true
    });
  }
}