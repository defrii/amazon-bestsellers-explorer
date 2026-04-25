import { Component } from '@angular/core';
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
  login = '';
  password = '';
  message = '';

  constructor(private http: HttpClient, private router: Router) {}

  loginUser() {
    this.message = '';
    const payload = { login: this.login, password: this.password };
    this.http.post('/api/auth/login', payload).subscribe({
      next: (res: any) => {
        if (res?.token) {
          localStorage.setItem('jwt', res.token);
          if (res.login) localStorage.setItem('login', res.login);
          this.message = 'Login successful';
          // redirect to home
          this.router.navigate(['/']);
        } else {
          this.message = 'Login succeeded, but no token received.';
        }
      },
      error: (err) => {
        this.message = err?.error || err?.error?.message || err?.statusText || 'Login failed';
      }
    });
  }
}
