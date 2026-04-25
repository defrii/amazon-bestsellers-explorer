import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router, RouterModule } from '@angular/router';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { MessageModule } from 'primeng/message';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    InputTextModule,
    PasswordModule,
    ButtonModule,
    CardModule,
    MessageModule
  ],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  login = '';
  password = '';
  message = '';
  isLoading = signal(false);

  constructor(private http: HttpClient, private router: Router) {}

  register() {
    this.message = '';
    this.isLoading.set(true);
    const payload = { login: this.login, password: this.password };
    this.http.post('/api/auth/register', payload).subscribe({
      next: (res: any) => {
        // Attempt to login automatically after registering
        const payload = { login: this.login, password: this.password };
        this.http.post('/api/auth/login', payload).subscribe({
          next: (loginRes: any) => {
            this.isLoading.set(false);
            if (loginRes?.token) {
              localStorage.setItem('jwt', loginRes.token);
              if (loginRes.login) localStorage.setItem('login', loginRes.login);
            }
            this.router.navigate(['/']);
          },
          error: () => {
            this.isLoading.set(false);
            // Even if auto-login fails, redirect to home
            this.router.navigate(['/']);
          }
        });
      },
      error: (err) => {
        this.isLoading.set(false);
        // Try to show server message if available
        if (typeof err?.error === 'string') {
          this.message = err.error;
        } else if (err?.error?.message) {
          this.message = err.error.message;
        } else if (err?.error?.title) {
          this.message = err.error.title;
        } else {
          this.message = err?.statusText || 'Registration failed';
        }
      }
    });
  }
}
