import { api } from "@/lib/axios";
import type { PagedResult } from "@/types/product";
import type {
  CreateSupplierRequest,
  SupplierDetails,
  SupplierListItem,
  UpdateSupplierRequest,
} from "@/types/supplier";

export async function getSuppliers(): Promise<PagedResult<SupplierListItem>> {
  const response = await api.get<PagedResult<SupplierListItem>>("/suppliers", {
    params: { pageSize: 100 },
  });

  return response.data;
}

export async function getSupplierById(id: string): Promise<SupplierDetails> {
  const response = await api.get<SupplierDetails>(`/suppliers/${id}`);

  return response.data;
}

export async function createSupplier(
  data: CreateSupplierRequest
): Promise<{ id: string }> {
  const response = await api.post<{ id: string }>("/suppliers", data);

  return response.data;
}

export async function updateSupplier(
  id: string,
  data: UpdateSupplierRequest
): Promise<void> {
  await api.put(`/suppliers/${id}`, data);
}

export async function deleteSupplier(id: string): Promise<void> {
  await api.delete(`/suppliers/${id}`);
}
