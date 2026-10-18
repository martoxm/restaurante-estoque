import { useQuery } from "@tanstack/react-query";
import { listarFornecedores } from "@/features/fornecedores/services/fornecedores";

export function useFornecedores() {
  return useQuery({
    queryKey: ["fornecedores"],
    queryFn: listarFornecedores,
  });
}
