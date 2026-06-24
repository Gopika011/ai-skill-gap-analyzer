import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5213/auth';
  private currentUserKey = 'app_current_user';

  private currentUserSubject = new BehaviorSubject<any>(this.getCurrentUser());
  currentUser$: Observable<any> = this.currentUserSubject.asObservable();

  login(data: any) {
    return this.http.post(`${this.apiUrl}/login`, data);
  }

  register(data: any) {
    return this.http.post(`${this.apiUrl}/register`, data);
  }

  getCurrentUser(): any {
    if (typeof window !== 'undefined' && window.localStorage) {
      const data = localStorage.getItem(this.currentUserKey);
      return data ? JSON.parse(data) : null;
    }
    return null;
  }

  setCurrentUser(user: any): void {
    if (typeof window !== 'undefined' && window.localStorage) {
      if (user) {
        localStorage.setItem(this.currentUserKey, JSON.stringify(user));
      } else {
        localStorage.removeItem(this.currentUserKey);
      }
    }
    this.currentUserSubject.next(user);
  }

  isLoggedIn(): boolean {
    return this.getCurrentUser() !== null;
  }

  logout(): void {
    this.setCurrentUser(null);
  }
}
