import { api } from "@/lib/axios";
import type { CreateStockMovementRequest } from "@/types/stock-movement";

export async function createStockMovement(
  data: CreateStockMovementRequest
): Promise<{ id: string }> {
  const response = await api.post<{ id: string }>("/stock-movements", data);

  return response.data;
}
