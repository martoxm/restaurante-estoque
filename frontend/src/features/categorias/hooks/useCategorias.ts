import { useQuery } from "@tanstack/react-query";
import { listarCategorias } from "@/features/categorias/services/categorias";

export function useCategorias() {
  return useQuery({
    queryKey: ["categorias"],
    queryFn: listarCategorias,
  });
}
