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
  favoriteAsins = signal<Set<string>>(new Set());
  isLoadingFavorite = signal<string | null>(null);
  error = '';

  constructor(private router: Router, private http: HttpClient) {}

  ngOnInit(): void {
    this.loadProducts();
    if (this.loggedUser !== 'none') {
      this.loadFavorites();
    }
  }

  goLogin() { this.router.navigate(['/login']); }
  goRegister() { this.router.navigate(['/register']); }
  logout() {
    localStorage.removeItem('jwt');
    localStorage.removeItem('login');
    this.loggedUser = 'none';
    this.favoriteAsins.set(new Set());
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

  private loadFavorites() {
    this.http.get<string[]>('/api/favorites').subscribe({
      next: (asins) => {
        this.favoriteAsins.set(new Set(asins));
      },
      error: (err) => console.error('Failed to load favorites', err)
    });
  }

  toggleFavorite(product: Product) {
    if (!product.asin) return;
    
    this.isLoadingFavorite.set(product.asin);
    const isFavorite = this.favoriteAsins().has(product.asin);
    
    if (isFavorite) {
      this.http.delete(`/api/favorites/${product.asin}`).subscribe({
        next: () => {
          const newSet = new Set(this.favoriteAsins());
          newSet.delete(product.asin!);
          this.favoriteAsins.set(newSet);
          this.isLoadingFavorite.set(null);
        },
        error: (err) => {
          console.error(err);
          this.isLoadingFavorite.set(null);
        }
      });
    } else {
      this.http.post('/api/favorites', product).subscribe({
        next: () => {
          const newSet = new Set(this.favoriteAsins());
          newSet.add(product.asin!);
          this.favoriteAsins.set(newSet);
          this.isLoadingFavorite.set(null);
        },
        error: (err) => {
          console.error(err);
          this.isLoadingFavorite.set(null);
        }
      });
    }
  }
}
