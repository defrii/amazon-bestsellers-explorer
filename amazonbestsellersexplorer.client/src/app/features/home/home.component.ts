import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { DataViewModule } from 'primeng/dataview';
import { RatingModule } from 'primeng/rating';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { CardModule } from 'primeng/card';
import { ToolbarModule } from 'primeng/toolbar';

// short model for product
type Product = { photo?: string; title?: string; price?: string; starRating?: string; url?: string; asin?: string };

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    DataViewModule,
    RatingModule,
    ProgressSpinnerModule,
    CardModule,
    ToolbarModule
  ],
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

  parseRating(rating?: string): number {
    if (!rating) return 0;
    const match = rating.match(/([0-9.]+)/);
    return match ? parseFloat(match[1]) : 0;
  }
}
