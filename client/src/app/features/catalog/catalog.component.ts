import { CurrencyPipe, NgFor, NgIf } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';
import { ProductApiService } from '../../core/services/product-api.service';
import { Product, ProductListResponse, ProductQuery } from '../../core/models/product.model';

@Component({
  selector: 'app-catalog',
  standalone: true,
  imports: [CurrencyPipe, FormsModule, NgFor, NgIf, RouterLink],
  templateUrl: './catalog.component.html',
  styleUrl: './catalog.component.scss',
})
export class CatalogComponent implements OnInit {
  private readonly productsApi = inject(ProductApiService);

  products: Product[] = [];
  brands: string[] = [];
  types: string[] = [];
  page = 1;
  readonly pageSize = 12;
  totalPages = 1;
  totalCount = 0;
  selectedBrand = '';
  selectedType = '';
  selectedSort: ProductQuery['sort'] = undefined;
  loading = true;
  filterLoading = true;
  errorMessage = '';

  ngOnInit(): void {
    this.loadFilters();
    this.loadProducts();
  }

  loadProducts(): void {
    this.loading = true;
    this.errorMessage = '';
    this.productsApi
      .list({
        brand: this.selectedBrand || undefined,
        type: this.selectedType || undefined,
        sort: this.selectedSort,
        page: this.page,
        pageSize: this.pageSize,
      })
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (response) => this.applyResponse(response),
        error: () => (this.errorMessage = 'KhÃ´ng thá»ƒ táº£i sáº£n pháº©m. HÃ£y kiá»ƒm tra API vÃ  thá»­ láº¡i.'),
      });
  }

  resetFilters(): void {
    this.selectedBrand = '';
    this.selectedType = '';
    this.selectedSort = undefined;
    this.page = 1;
    this.loadProducts();
  }

  onFilterChange(): void {
    this.page = 1;
    this.loadProducts();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages || page === this.page) return;
    this.page = page;
    this.loadProducts();
  }

  trackByProduct(_: number, product: Product): number {
    return product.id;
  }

  private loadFilters(): void {
    forkJoin({
      brands: this.productsApi.brands().pipe(catchError(() => of([]))),
      types: this.productsApi.types().pipe(catchError(() => of([]))),
    }).subscribe({
      next: ({ brands, types }) => {
        this.brands = brands;
        this.types = types;
        this.filterLoading = false;
      },
      error: () => (this.filterLoading = false),
    });
  }

  private applyResponse(response: ProductListResponse): void {
    this.products = response.items;
    this.totalCount = response.totalCount;
    this.totalPages = Math.max(response.totalPages, 1);
  }
}

