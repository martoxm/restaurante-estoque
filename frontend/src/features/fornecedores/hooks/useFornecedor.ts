import { useQuery } from "@tanstack/react-query";
import { obterFornecedorPorId } from "@/features/fornecedores/services/fornecedores";

export function useFornecedor(id: string) {
  return useQuery({
    queryKey: ["fornecedores", id],
    queryFn: () => obterFornecedorPorId(id),
    enabled: !!id,
  });
}
