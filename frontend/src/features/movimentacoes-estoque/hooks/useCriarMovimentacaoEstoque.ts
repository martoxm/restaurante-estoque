import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useRouter } from "next/navigation";
import { criarMovimentacaoEstoque } from "@/features/movimentacoes-estoque/services/movimentacoes-estoque";

export function useCriarMovimentacaoEstoque() {
  const router = useRouter();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: criarMovimentacaoEstoque,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["produtos"] });
      router.push("/produtos");
    },
  });
}
