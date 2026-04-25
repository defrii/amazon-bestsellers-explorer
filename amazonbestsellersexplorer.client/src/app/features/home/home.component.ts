import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {
  loggedUser = localStorage.getItem('login') || 'none';

  constructor(private router: Router) {}
  goLogin() { this.router.navigate(['/login']); }
  goRegister() { this.router.navigate(['/register']); }
  logout() {
    localStorage.removeItem('jwt');
    localStorage.removeItem('login');
    this.loggedUser = 'none';
    // stay on home; optionally reload
  }
}

