import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, HttpClientModule],
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

