import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { ToolbarModule } from 'primeng/toolbar';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, ButtonModule, ToolbarModule, RouterModule],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent {
  @Input() isFavorites = false;

  @Output() onLogin = new EventEmitter<void>();
  @Output() onRegister = new EventEmitter<void>();
  @Output() onFavorites = new EventEmitter<void>();

  constructor(public authService: AuthService, private router: Router) {}

  logout() {
    this.authService.logout();
  }
}
