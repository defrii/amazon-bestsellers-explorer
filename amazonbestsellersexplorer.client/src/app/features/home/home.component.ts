import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { ButtonModule } from 'primeng/button';
import { ToolbarModule } from 'primeng/toolbar';
import { FavoritesService } from '../../core/services/favorites.service';
import { ProductListComponent } from '../../shared/components/product-list/product-list.component';
import { NavbarComponent } from '../../shared/components/navbar/navbar.component';
import { Product } from '../../core/models/product.model';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    ButtonModule,
    ToolbarModule,
    ProductListComponent,
    NavbarComponent
  ],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  loggedUser = localStorage.getItem('login') || 'none';
  products = signal<Product[] | null>(null);
  error = '';

  constructor(
    private router: Router,
    private http: HttpClient,
    public favoritesService: FavoritesService
  ) {}

  ngOnInit(): void {
    this.loadProducts();
    if (this.loggedUser !== 'none') {
      this.favoritesService.loadFavoriteAsins();
    }
  }

  goLogin() { this.router.navigate(['/login']); }
  goRegister() { this.router.navigate(['/register']); }
  goFavorites() { this.router.navigate(['/favorites']); }

  logout() {
    localStorage.removeItem('jwt');
    localStorage.removeItem('login');
    this.loggedUser = 'none';
    this.favoritesService.favoriteAsins.set(new Set());
  }

  private loadProducts() {
    this.error = '';
    this.products.set(null);
    this.http.get<any>('/api/amazon/best-sellers').subscribe({
      next: (resp: any) => {
        this.products.set(Array.isArray(resp) ? resp : []);
      },
      error: (err) => {
        this.error = 'Nie udało się załadować produktów';
        this.products.set([]);
        console.error(err);
      }
    });
  }
}
