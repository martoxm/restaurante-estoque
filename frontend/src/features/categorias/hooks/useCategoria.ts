import { useQuery } from "@tanstack/react-query";
import { obterCategoriaPorId } from "@/features/categorias/services/categorias";

export function useCategoria(id: string) {
  return useQuery({
    queryKey: ["categorias", id],
    queryFn: () => obterCategoriaPorId(id),
    enabled: !!id,
  });
}
