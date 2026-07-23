import { CurrencyPipe, NgFor, NgIf } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AdminKeyService } from '../../core/services/admin-key.service';
import { ProductApiService } from '../../core/services/product-api.service';
import { Product } from '../../core/models/product.model';

interface ProductForm {
  name: string;
  description: string;
  price: number;
  pictureUrl: string;
  brand: string;
  type: string;
  quantity: number;
  isVisible: boolean;
}

const emptyForm = (): ProductForm => ({ name: '', description: '', price: 0, pictureUrl: '', brand: '', type: '', quantity: 0, isVisible: true });

@Component({
  selector: 'app-admin-products',
  standalone: true,
  imports: [CurrencyPipe, FormsModule, NgFor, NgIf, RouterLink],
  templateUrl: './admin-products.component.html',
  styleUrl: './admin-products.component.scss',
})
export class AdminProductsComponent implements OnInit {
  private readonly api = inject(ProductApiService);
  readonly adminKey = inject(AdminKeyService);
  products: Product[] = [];
  form = emptyForm();
  editingId: number | null = null;
  keyInput = '';
  loading = false;
  message = '';
  errorMessage = '';

  ngOnInit(): void { this.keyInput = this.adminKey.key; this.loadProducts(); }

  saveKey(): void { this.adminKey.setKey(this.keyInput); this.message = 'Admin key saved for this browser session.'; this.loadProducts(); }
  loadProducts(): void {
    this.loading = true; this.errorMessage = '';
    this.api.list({ page: 1, pageSize: 100 }).subscribe({
      next: (response) => { this.products = response.items; this.loading = false; },
      error: () => { this.errorMessage = 'Could not load products. Check the API and admin key.'; this.loading = false; },
    });
  }
  edit(product: Product): void {
    this.editingId = product.id;
    this.form = { name: product.name, description: product.description, price: product.price, pictureUrl: product.pictureUrl, brand: product.brand ?? '', type: product.type ?? '', quantity: product.quantityInStock, isVisible: product.isVisible };
  }
  cancelEdit(): void { this.editingId = null; this.form = emptyForm(); }
  submit(): void {
    if (!this.form.name.trim()) return;
    const request = this.editingId === null ? this.api.create(this.form) : this.api.update(this.editingId, this.form);
    request.subscribe({ next: () => { this.message = this.editingId === null ? 'Product created.' : 'Product updated.'; this.cancelEdit(); this.loadProducts(); }, error: () => this.errorMessage = 'Save failed. Check the admin key and required fields.' });
  }
  remove(product: Product): void {
    if (!confirm(`Delete ${product.name}?`)) return;
    this.api.delete(product.id).subscribe({ next: () => { this.message = 'Product deleted.'; this.loadProducts(); }, error: () => this.errorMessage = 'Delete failed. Check the admin key.' });
  }
}
