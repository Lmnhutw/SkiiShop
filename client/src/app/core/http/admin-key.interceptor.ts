import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AdminKeyService } from '../services/admin-key.service';

export const adminKeyInterceptor: HttpInterceptorFn = (request, next) => {
  const key = inject(AdminKeyService).key;
  if (!key || !request.url.includes('/products')) return next(request);
  return next(request.clone({ setHeaders: { 'X-Admin-Key': key } }));
};
