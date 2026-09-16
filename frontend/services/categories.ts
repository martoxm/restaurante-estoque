import { api } from "@/lib/axios";
import type { PagedResult } from "@/types/product";
import type {
  CategoryDetails,
  CategoryListItem,
  CreateCategoryRequest,
  UpdateCategoryRequest,
} from "@/types/category";

export async function getCategories(): Promise<PagedResult<CategoryListItem>> {
  const response = await api.get<PagedResult<CategoryListItem>>("/categories", {
    params: { pageSize: 100 },
  });

  return response.data;
}

export async function getCategoryById(id: string): Promise<CategoryDetails> {
  const response = await api.get<CategoryDetails>(`/categories/${id}`);

  return response.data;
}

export async function createCategory(
  data: CreateCategoryRequest
): Promise<{ id: string }> {
  const response = await api.post<{ id: string }>("/categories", data);

  return response.data;
}

export async function updateCategory(
  id: string,
  data: UpdateCategoryRequest
): Promise<void> {
  await api.put(`/categories/${id}`, data);
}

export async function deleteCategory(id: string): Promise<void> {
  await api.delete(`/categories/${id}`);
}
