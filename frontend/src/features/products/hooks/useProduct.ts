import { useQuery } from "@tanstack/react-query";
import { getProductById } from "@/features/products/services/products";

export function useProduct(id: string) {
  return useQuery({
    queryKey: ["products", id],
    queryFn: () => getProductById(id),
    enabled: !!id,
  });
}
