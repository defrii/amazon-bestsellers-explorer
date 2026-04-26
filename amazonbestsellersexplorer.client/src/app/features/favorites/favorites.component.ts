import { Component, OnInit, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { ToolbarModule } from 'primeng/toolbar';
import { AmazonService } from '../../core/services/amazon.service';
import { AuthService } from '../../core/services/auth.service';
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
  products = signal<Product[] | null>(null);
  error = '';

  constructor(
    private router: Router,
    public authService: AuthService,
    public amazonService: AmazonService
  ) {
    effect(() => {
      if (!this.authService.isAuthenticated()) {
        this.router.navigate(['/login']);
      }
    });
  }

  ngOnInit(): void {
    if (!this.authService.isAuthenticated()) {
      this.router.navigate(['/login']);
      return;
    }
    this.loadFavorites();
  }

  private loadFavorites() {
    this.products.set(null);
    this.error = '';
    this.amazonService.getFavoriteDetails().subscribe({
      next: (items) => {
        this.products.set(items);
        const asins = new Set(items.map(i => i.asin!).filter(Boolean));
        this.amazonService.favoriteAsins.set(asins);
      },
      error: (err) => {
        this.error = 'Nie udało się załadować ulubionych produktów.';
        this.products.set([]);
        console.error(err);
      }
    });
  }

  handleToggle(product: Product) {
    if (!product.asin) return;
    const asin = product.asin;

    this.amazonService.isLoadingFavorite.set(asin);
    this.amazonService.removeFavorite(asin).subscribe({
      next: () => {
        const newSet = new Set(this.amazonService.favoriteAsins());
        newSet.delete(asin);
        this.amazonService.favoriteAsins.set(newSet);
        this.amazonService.isLoadingFavorite.set(null);
        const updated = (this.products() ?? []).filter(p => p.asin !== asin);
        this.products.set(updated);
      },
      error: () => this.amazonService.isLoadingFavorite.set(null)
    });
  }

  goHome() { this.router.navigate(['/']); }
  logout() { this.authService.logout(); }
}
