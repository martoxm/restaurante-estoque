import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useRouter } from "next/navigation";
import { criarFornecedor } from "@/features/fornecedores/services/fornecedores";

export function useCriarFornecedor() {
  const router = useRouter();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: criarFornecedor,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["fornecedores"] });
      router.push("/fornecedores");
    },
  });
}
