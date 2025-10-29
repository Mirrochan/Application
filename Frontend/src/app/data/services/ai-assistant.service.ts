import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { aiAsistantResponce, quetionModel } from '../interfaces/ai.model';

@Injectable({
  providedIn: 'root'
})
export class AiAssistantService {
  private url = 'http://localhost:5107'; 

  constructor(private http: HttpClient) { }

  askQuestion(question: quetionModel): Observable<aiAsistantResponce> {
    return this.http.post<aiAsistantResponce>(`${this.url}/ai-assistant/ask`, question,{
      withCredentials: true
    });
  }

  getConversationHistory(): Observable<aiAsistantResponce[]> {
    return this.http.get<aiAsistantResponce[]>(`${this.url}/history`,{
      withCredentials: true
    });
  }
}