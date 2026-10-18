import { api } from "@/lib/axios";
import type { RestauranteDetalhes } from "@/features/restaurantes/types/restaurante";

export async function obterRestaurantePorId(id: string): Promise<RestauranteDetalhes> {
  const response = await api.get<RestauranteDetalhes>(`/restaurantes/${id}`);

  return response.data;
}
