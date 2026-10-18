import { useMutation, useQueryClient } from "@tanstack/react-query";
import { excluirCategoria } from "@/features/categorias/services/categorias";

export function useExcluirCategoria() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: excluirCategoria,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["categorias"] });
    },
  });
}
