import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { Product } from '../../../core/models/product.model';

@Component({
  selector: 'app-product-card',
  standalone: true,
  imports: [CommonModule, ButtonModule, CardModule],
  templateUrl: './product-card.component.html',
  styleUrls: ['./product-card.component.scss']
})
export class ProductCardComponent {
  @Input() product!: Product;
  @Input() isFavorite = false;
  @Input() isLoadingFavorite: string | null = null;
  @Input() loggedUser = 'none';

  @Output() toggleFavoriteAction = new EventEmitter<Product>();

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
