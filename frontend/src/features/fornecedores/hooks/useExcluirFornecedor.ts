import { useMutation, useQueryClient } from "@tanstack/react-query";
import { excluirFornecedor } from "@/features/fornecedores/services/fornecedores";

export function useExcluirFornecedor() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: excluirFornecedor,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["fornecedores"] });
    },
  });
}
