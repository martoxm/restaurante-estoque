export interface CategoryListItem {
  id: string;
  name: string;
  description?: string | null;
  productCount: number;
}

export interface CategoryDetails {
  id: string;
  name: string;
  description?: string | null;
  productCount: number;
}

export interface CreateCategoryRequest {
  name: string;
  description?: string;
}

export interface UpdateCategoryRequest {
  name: string;
  description?: string;
}
