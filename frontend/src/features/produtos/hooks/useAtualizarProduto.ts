import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useRouter } from "next/navigation";
import { atualizarProduto } from "@/features/produtos/services/produtos";
import type { AtualizarProdutoRequest } from "@/features/produtos/types/produto";

export function useAtualizarProduto() {
  const router = useRouter();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: AtualizarProdutoRequest }) =>
      atualizarProduto(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["produtos"] });
      router.push("/produtos");
    },
  });
}
