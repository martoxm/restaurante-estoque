import { useQuery } from "@tanstack/react-query";
import { getSupplierById } from "@/services/suppliers";

export function useSupplier(id: string) {
  return useQuery({
    queryKey: ["suppliers", id],
    queryFn: () => getSupplierById(id),
    enabled: !!id,
  });
}
