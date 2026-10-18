import { useQuery } from "@tanstack/react-query";
import { obterProdutoPorId } from "@/features/produtos/services/produtos";

export function useProduto(id: string) {
  return useQuery({
    queryKey: ["produtos", id],
    queryFn: () => obterProdutoPorId(id),
    enabled: !!id,
  });
}
