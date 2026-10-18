import { useQuery } from "@tanstack/react-query";
import { listarProdutos, type ListarProdutosParams } from "@/features/produtos/services/produtos";

export function useProdutos(params: ListarProdutosParams = {}) {
  return useQuery({
    queryKey: ["produtos", params],
    queryFn: () => listarProdutos(params),
  });
}
