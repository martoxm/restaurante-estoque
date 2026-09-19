import { useQuery } from "@tanstack/react-query";
import { getRestaurantById } from "@/features/restaurants/services/restaurants";

export function useRestaurant(id: string | undefined) {
  return useQuery({
    queryKey: ["restaurants", id],
    queryFn: () => getRestaurantById(id as string),
    enabled: !!id,
  });
}
