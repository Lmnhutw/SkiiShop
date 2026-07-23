import { Routes } from '@angular/router';
import { CatalogComponent } from './features/catalog/catalog.component';
import { ProductDetailComponent } from './features/product-detail/product-detail.component';
import { AdminProductsComponent } from './features/admin/admin-products.component';
import { CartComponent } from './features/cart/cart.component';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'products' },
  { path: 'products', component: CatalogComponent },
  { path: 'products/:id', component: ProductDetailComponent },
  { path: 'cart', component: CartComponent },
  { path: 'admin/products', component: AdminProductsComponent },
  { path: '**', redirectTo: 'products' },
];
