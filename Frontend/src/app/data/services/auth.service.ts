import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, catchError, map, Observable, of, tap } from 'rxjs';
import { LoginRequest, RegisterRequest } from '../interfaces/auth.model';


@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly API_URL = 'http://localhost:5107';
  private isLoggedInSubject = new BehaviorSubject<boolean>(false);
  public isLoggedIn$ = this.isLoggedInSubject.asObservable();
  constructor(private http: HttpClient) {this.checkAuth().subscribe();}

  login(credentials: LoginRequest): Observable<any> {
    return this.http.post(`${this.API_URL}/auth/login`, credentials, { 
      withCredentials: true 
    }).pipe(
      tap(() => this.isLoggedInSubject.next(true))
    );
  }

  register(userData: RegisterRequest): Observable<any> {
    return this.http.post(`${this.API_URL}/auth/register`, userData, {
      withCredentials: true
    });
  }

  logout(): Observable<any> {
    return this.http.post(`${this.API_URL}/auth/logout`, {}, {
      withCredentials: true
    }).pipe(
      tap(() => this.isLoggedInSubject.next(false))
    );
  }

  isLoggedIn(): boolean {
    return this.isLoggedInSubject.value;
  }
  checkAuth(): Observable<boolean> {
  return this.http.get(`${this.API_URL}/auth/check`, { withCredentials: true }).pipe(
    map(() => {
      this.isLoggedInSubject.next(true);
      return true;
    }),
    catchError(() => {
      this.isLoggedInSubject.next(false);
      return of(false);
    })
  );
}
getName(): Observable<{ name: string }> {
  return this.http.get<{ name: string }>(`${this.API_URL}/users/me`, {
    withCredentials: true
  });
}}
