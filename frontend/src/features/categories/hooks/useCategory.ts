import { useQuery } from "@tanstack/react-query";
import { getCategoryById } from "@/features/categories/services/categories";

export function useCategory(id: string) {
  return useQuery({
    queryKey: ["categories", id],
    queryFn: () => getCategoryById(id),
    enabled: !!id,
  });
}
