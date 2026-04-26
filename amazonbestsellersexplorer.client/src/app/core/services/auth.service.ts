import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly JWT_KEY = 'jwt';
  private readonly LOGIN_KEY = 'login';

  currentUser = signal<string | null>(localStorage.getItem(this.LOGIN_KEY));
  isAuthenticated = computed(() => !!this.currentUser());

  constructor(private http: HttpClient, private router: Router) {}

  login(credentials: any) {
    return this.http.post<any>('/api/auth/login', credentials).pipe(
      tap(res => {
        if (res?.token) {
          localStorage.setItem(this.JWT_KEY, res.token);
          if (res.login) {
            localStorage.setItem(this.LOGIN_KEY, res.login);
            this.currentUser.set(res.login);
          }
        }
      })
    );
  }

  register(userData: any) {
    return this.http.post<any>('/api/auth/register', userData);
  }

  logout() {
    localStorage.removeItem(this.JWT_KEY);
    localStorage.removeItem(this.LOGIN_KEY);
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }

  // Metoda pomocnicza dla interceptora lub innych serwisów
  clearSession() {
    localStorage.removeItem(this.JWT_KEY);
    localStorage.removeItem(this.LOGIN_KEY);
    this.currentUser.set(null);
  }
}
