import { api } from "@/lib/axios";
import type { RestaurantDetails } from "@/features/restaurants/types/restaurant";

export async function getRestaurantById(id: string): Promise<RestaurantDetails> {
  const response = await api.get<RestaurantDetails>(`/restaurants/${id}`);

  return response.data;
}
