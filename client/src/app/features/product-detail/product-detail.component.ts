import { CurrencyPipe, NgIf } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ProductApiService } from '../../core/services/product-api.service';
import { Product } from '../../core/models/product.model';
import { CartService } from '../../core/services/cart.service';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CurrencyPipe, NgIf, RouterLink],
  templateUrl: './product-detail.component.html',
  styleUrl: './product-detail.component.scss',
})
export class ProductDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly productsApi = inject(ProductApiService);
  readonly cart = inject(CartService);
  product: Product | null = null;
  loading = true;
  errorMessage = '';
  addedMessage = '';

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!id) { this.loading = false; this.errorMessage = 'Product not found.'; return; }
    this.productsApi.getById(id).subscribe({
      next: (product) => { this.product = product; this.loading = false; },
      error: () => { this.errorMessage = 'Không tìm thấy sản phẩm này.'; this.loading = false; },
    });
  }

  addToCart(): void {
    if (!this.product || this.product.quantityInStock === 0) return;
    this.cart.add(this.product);
    this.addedMessage = 'Added to your cart.';
  }
}
