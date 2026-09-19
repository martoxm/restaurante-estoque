import { useQuery } from "@tanstack/react-query";
import { getProducts, type GetProductsParams } from "@/features/products/services/products";

export function useProducts(params: GetProductsParams = {}) {
  return useQuery({
    queryKey: ["products", params],
    queryFn: () => getProducts(params),
  });
}
