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
  login = signal('');
  password = signal('');
  message = signal('');
  isLoading = signal(false);

  constructor(private http: HttpClient, private router: Router) {}

  register() {
    this.message.set('');
    this.isLoading.set(true);
    this.http.post('/api/auth/register', { login: this.login(), password: this.password() }).subscribe({
      next: (res: any) => {
        this.http.post('/api/auth/login', { login: this.login(), password: this.password() }).subscribe({
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
            this.router.navigate(['/']);
          }
        });
      },
      error: (err) => {
        this.isLoading.set(false);
        if (typeof err?.error === 'string') {
          this.message.set(err.error);
        } else if (err?.error?.message) {
          this.message.set(err.error.message);
        } else if (err?.error?.title) {
          this.message.set(err.error.title);
        } else {
          this.message.set(err?.statusText || 'Registration failed');
        }
      }
    });
  }
}
