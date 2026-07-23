import { computed, Injectable, signal } from '@angular/core';
import { Product } from '../models/product.model';

export interface CartItem {
  product: Product;
  quantity: number;
}

const STORAGE_KEY = 'skiishop.cart';

@Injectable({ providedIn: 'root' })
export class CartService {
  readonly items = signal<CartItem[]>(this.read());
  readonly itemCount = computed(() => this.items().reduce((sum, item) => sum + item.quantity, 0));
  readonly subtotal = computed(() => this.items().reduce((sum, item) => sum + item.product.price * item.quantity, 0));

  add(product: Product): void {
    const current = this.items();
    const existing = current.find((item) => item.product.id === product.id);
    const next = existing
      ? current.map((item) => item.product.id === product.id ? { ...item, quantity: item.quantity + 1 } : item)
      : [...current, { product, quantity: 1 }];
    this.update(next);
  }

  decrease(productId: number): void {
    this.update(this.items().flatMap((item) => {
      if (item.product.id !== productId) return [item];
      return item.quantity > 1 ? [{ ...item, quantity: item.quantity - 1 }] : [];
    }));
  }

  clear(): void { this.update([]); }

  private update(items: CartItem[]): void {
    this.items.set(items);
    localStorage.setItem(STORAGE_KEY, JSON.stringify(items));
  }

  private read(): CartItem[] {
    try { return JSON.parse(localStorage.getItem(STORAGE_KEY) ?? '[]') as CartItem[]; }
    catch { return []; }
  }
}
