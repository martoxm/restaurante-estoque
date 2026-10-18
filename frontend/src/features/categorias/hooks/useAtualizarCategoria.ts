import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useRouter } from "next/navigation";
import { atualizarCategoria } from "@/features/categorias/services/categorias";
import type { AtualizarCategoriaRequest } from "@/features/categorias/types/categoria";

export function useAtualizarCategoria() {
  const router = useRouter();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: AtualizarCategoriaRequest }) =>
      atualizarCategoria(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["categorias"] });
      router.push("/categorias");
    },
  });
}
