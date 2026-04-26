import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Product } from '../models/product.model';

@Injectable({ providedIn: 'root' })
export class FavoritesService {
  favoriteAsins = signal<Set<string>>(new Set());
  isLoadingFavorite = signal<string | null>(null);

  constructor(private http: HttpClient) {}

  loadFavoriteAsins() {
    this.http.get<string[]>('/api/favorites').subscribe({
      next: (asins) => this.favoriteAsins.set(new Set(asins)),
      error: (err) => console.error('Failed to load favorites', err)
    });
  }

  getFavoriteDetails() {
    return this.http.get<Product[]>('/api/favorites/details');
  }

  removeFavorite(asin: string) {
    return this.http.delete(`/api/favorites/${asin}`);
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
