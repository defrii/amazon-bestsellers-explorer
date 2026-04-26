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
  selector: 'app-login',
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
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  login = signal('');
  password = signal('');
  message = signal('');
  isLoading = signal(false);

  constructor(private http: HttpClient, private router: Router) {}

  loginUser() {
    this.message.set('');
    this.http.post('/api/auth/login', { login: this.login(), password: this.password() }).subscribe({
      next: (res: any) => {
        this.isLoading.set(false);
        if (res?.token) {
          localStorage.setItem('jwt', res.token);
          if (res.login) localStorage.setItem('login', res.login);
          this.router.navigate(['/']);
        } else {
          this.message.set('Login succeeded, but no token received.');
        }
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
          this.message.set(err?.statusText || 'Login failed');
        }
      }
    });
  }
}
