import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';

// short model for product
type Product = { photo?: string; title?: string; price?: string; starRating?: string; url?: string; asin?: string };

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  loggedUser = localStorage.getItem('login') || 'none';
  products = signal<Product[] | null>(null);
  error = '';

  constructor(private router: Router, private http: HttpClient) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  goLogin() { this.router.navigate(['/login']); }
  goRegister() { this.router.navigate(['/register']); }
  logout() {
    localStorage.removeItem('jwt');
    localStorage.removeItem('login');
    this.loggedUser = 'none';
  }

  private loadProducts() {
    this.error = '';
    this.products.set(null);
    this.http.get<any>('/api/amazon/best-sellers').subscribe({
      next: (resp: any) => {
        this.products.set(Array.isArray(resp) ? resp : []);
      },
      error: (err) => {
        this.error = 'Failed to load products';
        this.products.set([]);
        console.error(err);
      }
    });
  }

  openUrl(url?: string) {
    if (!url) return;
    window.open(url, '_blank');
  }
}

