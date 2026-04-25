import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HttpClient } from '@angular/common/http';

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

  constructor(private http: HttpClient) {}

  register() {
    this.message = '';
    const payload = { login: this.login, password: this.password };
    this.http.post('/api/auth/register', payload).subscribe({
      next: (res: any) => {
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

