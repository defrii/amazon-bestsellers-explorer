import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { MessageModule } from 'primeng/message';
import { AuthService } from '../../../core/services/auth.service';

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

  constructor(private authService: AuthService, private router: Router) { }

  register() {
    this.message.set('');
    this.isLoading.set(true);
    const credentials = { login: this.login(), password: this.password() };

    this.authService.register(credentials).subscribe({
      next: () => {
        this.authService.login(credentials).subscribe({
          next: () => {
            this.isLoading.set(false);
            this.router.navigate(['/']);
          },
          error: () => {
            this.isLoading.set(false);
            this.router.navigate(['/login']);
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
