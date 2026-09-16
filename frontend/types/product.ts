export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface ProductListItem {
  id: string;
  name: string;
  price: number;
  quantityInStock: number;
  categoryId: string;
  categoryName: string;
}

export interface CreateProductRequest {
  name: string;
  description?: string;
  price: number;
  categoryId: string;
  initialQuantity: number;
}

export interface ProductDetails {
  id: string;
  name: string;
  description?: string | null;
  price: number;
  quantityInStock: number;
  categoryId: string;
  categoryName: string;
}

export interface UpdateProductRequest {
  name: string;
  description?: string;
  price: number;
  categoryId: string;
}
