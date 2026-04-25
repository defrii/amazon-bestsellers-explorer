import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, HttpClientModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  login = '';
  password = '';
  message = '';

  constructor(private http: HttpClient, private router: Router) {}

  register() {
    this.message = '';
    const payload = { login: this.login, password: this.password };
    this.http.post('/api/auth/register', payload).subscribe({
      next: (res: any) => {
        // Attempt to login automatically after registering
        const payload = { login: this.login, password: this.password };
        this.http.post('/api/auth/login', payload).subscribe({
          next: (loginRes: any) => {
            if (loginRes?.token) {
              localStorage.setItem('jwt', loginRes.token);
              if (loginRes.login) localStorage.setItem('login', loginRes.login);
            }
            this.router.navigate(['/']);
          },
          error: () => {
            // Even if auto-login fails, redirect to home
            this.router.navigate(['/']);
          }
        });
        this.message = 'Registration successful';
        this.login = '';
        this.password = '';
      },
      error: (err) => {
        // Try to show server message if available
        this.message = err?.error || err?.error?.message || err?.statusText || 'Registration failed';
      }
    });
  }
}

