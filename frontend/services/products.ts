import { api } from "@/lib/axios";
import type {
  CreateProductRequest,
  PagedResult,
  ProductDetails,
  ProductListItem,
  UpdateProductRequest,
} from "@/types/product";

export interface GetProductsParams {
  page?: number;
  pageSize?: number;
  name?: string;
  categoryId?: string;
}

export async function getProducts(
  params: GetProductsParams = {}
): Promise<PagedResult<ProductListItem>> {
  const response = await api.get<PagedResult<ProductListItem>>("/products", {
    params,
  });

  return response.data;
}

export async function createProduct(
  data: CreateProductRequest
): Promise<{ id: string }> {
  const response = await api.post<{ id: string }>("/products", data);

  return response.data;
}

export async function getProductById(id: string): Promise<ProductDetails> {
  const response = await api.get<ProductDetails>(`/products/${id}`);

  return response.data;
}

export async function updateProduct(
  id: string,
  data: UpdateProductRequest
): Promise<void> {
  await api.put(`/products/${id}`, data);
}

export async function deleteProduct(id: string): Promise<void> {
  await api.delete(`/products/${id}`);
}
