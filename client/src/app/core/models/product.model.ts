export interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
  pictureUrl: string;
  brand: string | null;
  type: string | null;
  quantityInStock: number;
  isVisible: boolean;
  createdAt?: string;
  updateAt?: string;
}

export interface ProductListResponse {
  items: Product[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface ProductQuery {
  brand?: string;
  type?: string;
  sort?: 'priceasc' | 'pricedesc' | 'nameasc' | 'namedesc';
  page: number;
  pageSize: number;
}
