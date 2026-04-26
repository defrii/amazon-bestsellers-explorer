import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { ToolbarModule } from 'primeng/toolbar';
import { FavoritesService } from '../../core/services/favorites.service';
import { ProductListComponent } from '../../shared/components/product-list/product-list.component';
import { NavbarComponent } from '../../shared/components/navbar/navbar.component';
import { Product } from '../../core/models/product.model';

@Component({
  selector: 'app-favorites',
  standalone: true,
  imports: [CommonModule, ButtonModule, ToolbarModule, ProductListComponent, NavbarComponent],
  templateUrl: './favorites.component.html',
  styleUrls: ['./favorites.component.scss']
})
export class FavoritesComponent implements OnInit {
  loggedUser = localStorage.getItem('login') || 'none';
  products = signal<Product[] | null>(null);
  error = '';

  constructor(
    private router: Router,
    public favoritesService: FavoritesService
  ) {}

  ngOnInit(): void {
    if (this.loggedUser === 'none') {
      this.router.navigate(['/login']);
      return;
    }
    this.loadFavorites();
  }

  private loadFavorites() {
    this.products.set(null);
    this.error = '';
    this.favoritesService.getFavoriteDetails().subscribe({
      next: (items) => {
        this.products.set(items);
        // Populate favoriteAsins from the loaded items
        const asins = new Set(items.map(i => i.asin!).filter(Boolean));
        this.favoritesService.favoriteAsins.set(asins);
      },
      error: (err) => {
        this.error = 'Failed to load favorites';
        this.products.set([]);
        console.error(err);
      }
    });
  }

  handleToggle(product: Product) {
    if (!product.asin) return;
    const asin = product.asin;

    this.favoritesService.isLoadingFavorite.set(asin);
    this.favoritesService.removeFavorite(asin).subscribe({
      next: () => {
        const newSet = new Set(this.favoritesService.favoriteAsins());
        newSet.delete(asin);
        this.favoritesService.favoriteAsins.set(newSet);
        this.favoritesService.isLoadingFavorite.set(null);
        // Remove from local list immediately
        const updated = (this.products() ?? []).filter(p => p.asin !== asin);
        this.products.set(updated);
      },
      error: () => this.favoritesService.isLoadingFavorite.set(null)
    });
  }

  goHome() { this.router.navigate(['/']); }

  logout() {
    localStorage.removeItem('jwt');
    localStorage.removeItem('login');
    this.router.navigate(['/']);
  }
}
