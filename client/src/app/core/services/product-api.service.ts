import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../config/api.config';
import { ProductWritePayload } from '../models/product-write.model';
import { Product, ProductListResponse, ProductQuery } from '../models/product.model';

@Injectable({ providedIn: 'root' })
export class ProductApiService {
  private readonly http = inject(HttpClient);
  private readonly productsUrl = `${API_BASE_URL}/products`;

  list(query: ProductQuery): Observable<ProductListResponse> {
    let params = new HttpParams().set('page', query.page).set('pageSize', query.pageSize);
    if (query.brand) params = params.set('brand', query.brand);
    if (query.type) params = params.set('type', query.type);
    if (query.sort) params = params.set('sort', query.sort);
    return this.http.get<ProductListResponse>(this.productsUrl, { params });
  }

  getById(id: number): Observable<Product> {
    return this.http.get<Product>(this.productsUrl, { params: { id } });
  }

  create(payload: ProductWritePayload): Observable<Product> {
    return this.http.post<Product>(`${this.productsUrl}/create`, payload);
  }

  update(id: number, payload: ProductWritePayload): Observable<Product> {
    return this.http.put<Product>(`${this.productsUrl}/updates/${id}`, payload);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.productsUrl}/delete/${id}`);
  }

  brands(): Observable<string[]> {
    return this.http.get<string[]>(`${this.productsUrl}/brands`);
  }

  types(): Observable<string[]> {
    return this.http.get<string[]>(`${this.productsUrl}/types`);
  }
}
