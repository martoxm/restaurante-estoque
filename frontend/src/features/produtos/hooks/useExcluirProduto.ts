import { useMutation, useQueryClient } from "@tanstack/react-query";
import { excluirProduto } from "@/features/produtos/services/produtos";

export function useExcluirProduto() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: excluirProduto,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["produtos"] });
    },
  });
}
