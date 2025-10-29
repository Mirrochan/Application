import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";

@Injectable({
  providedIn: 'root'
})
export class TagService {
  private readonly API_URL = 'http://localhost:5107';

  constructor(private http: HttpClient) { }
  getAllTags() {
    return this.http.get(`${this.API_URL}/tags/getAllTags`, {
      withCredentials: true
    });
  }
}