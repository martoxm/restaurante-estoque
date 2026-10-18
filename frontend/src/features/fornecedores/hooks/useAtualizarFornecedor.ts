import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useRouter } from "next/navigation";
import { atualizarFornecedor } from "@/features/fornecedores/services/fornecedores";
import type { AtualizarFornecedorRequest } from "@/features/fornecedores/types/fornecedor";

export function useAtualizarFornecedor() {
  const router = useRouter();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: AtualizarFornecedorRequest }) =>
      atualizarFornecedor(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["fornecedores"] });
      router.push("/fornecedores");
    },
  });
}
