import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useRouter } from "next/navigation";
import { criarProduto } from "@/features/produtos/services/produtos";

export function useCriarProduto() {
  const router = useRouter();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: criarProduto,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["produtos"] });
      router.push("/produtos");
    },
  });
}
