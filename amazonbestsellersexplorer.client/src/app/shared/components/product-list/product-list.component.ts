import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DataViewModule } from 'primeng/dataview';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { ProductCardComponent } from '../product-card/product-card.component';
import { Product } from '../../../core/models/product.model';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, DataViewModule, ProgressSpinnerModule, ProductCardComponent],
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss']
})
export class ProductListComponent {
  @Input() products: Product[] | null = null;
  @Input() favoriteAsins: Set<string> = new Set();
  @Input() isLoadingFavorite: string | null = null;
  @Input() loggedUser = 'none';
  @Input() error = '';
  @Input() emptyMessage = 'Nie znaleziono produktów.';

  @Output() toggleFavoriteAction = new EventEmitter<Product>();
}
