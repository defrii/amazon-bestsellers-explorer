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
  products = signal<Product[] | null>(null);
  error = '';

  constructor(
    private router: Router,
    public amazonService: AmazonService,
    public authService: AuthService
  ) { 
    // Automatycznie ładuj ulubione gdy użytkownik się zaloguje
    effect(() => {
      if (this.authService.isAuthenticated()) {
        this.amazonService.loadFavoriteAsins();
      } else {
        this.amazonService.favoriteAsins.set(new Set());
      }
    });
  }

  ngOnInit(): void {
    this.loadProducts();
  }

  goLogin() { this.router.navigate(['/login']); }
  goRegister() { this.router.navigate(['/register']); }
  goFavorites() { this.router.navigate(['/favorites']); }
  logout() { this.authService.logout(); }

  private loadProducts() {
    this.error = '';
    this.products.set(null);
    this.amazonService.getBestSellers().subscribe({
      next: (resp) => {
        this.products.set(resp);
      },
      error: (err) => {
        this.error = 'Nie udało się załadować produktów';
        this.products.set([]);
        console.error(err);
      }
    });
  }
}
