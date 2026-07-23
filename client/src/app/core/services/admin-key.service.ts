import { Injectable } from '@angular/core';

const STORAGE_KEY = 'skiishop.adminKey';

@Injectable({ providedIn: 'root' })
export class AdminKeyService {
  get key(): string {
    return sessionStorage.getItem(STORAGE_KEY) ?? '';
  }

  setKey(key: string): void {
    if (key.trim()) sessionStorage.setItem(STORAGE_KEY, key.trim());
    else sessionStorage.removeItem(STORAGE_KEY);
  }

  clear(): void {
    sessionStorage.removeItem(STORAGE_KEY);
  }
}
