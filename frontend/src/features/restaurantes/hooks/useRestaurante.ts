import { useQuery } from "@tanstack/react-query";
import { obterRestaurantePorId } from "@/features/restaurantes/services/restaurantes";

export function useRestaurante(id: string | undefined) {
  return useQuery({
    queryKey: ["restaurantes", id],
    queryFn: () => obterRestaurantePorId(id as string),
    enabled: !!id,
  });
}
