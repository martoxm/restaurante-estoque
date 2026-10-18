import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useRouter } from "next/navigation";
import { criarCategoria } from "@/features/categorias/services/categorias";

export function useCriarCategoria() {
  const router = useRouter();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: criarCategoria,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["categorias"] });
      router.push("/categorias");
    },
  });
}
